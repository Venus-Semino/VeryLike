# Guía completa: VeryLike en AWS

Esta guía deja la aplicación disponible aunque la computadora local esté
apagada. La base de datos y los contenedores viven en AWS; la computadora solo
se usa para editar código y hacer `git push`.

## Arquitectura elegida

```text
Internet -> Application Load Balancer -> ECS/Fargate: VeryLike.Web
                                           |
                      Cloud Map -> Catalog.API y Forum.API (privadas)
                                           |
                                  Aurora PostgreSQL Serverless v2 (privada)
```

Las imágenes de los tres servicios se guardan en Amazon ECR. GitHub Actions
las construye en cada `push` a la rama `pipeline-ci` y actualiza los servicios
de ECS.

## Estado actual

- [x] Código adaptado de SQL Server/LocalDB a PostgreSQL con Npgsql.
- [x] Migraciones verificadas con SQL compatible con PostgreSQL.
- [x] Dockerfiles para Web, Catalog.API y Forum.API.
- [x] Health checks en `/health`.
- [x] Workflow de despliegue en `.github/workflows/deploy-aws.yml`.
- [x] Aurora PostgreSQL Serverless v2 creada en `us-east-2`, privada, con la
  base inicial `verylike`, mínimo 0 ACU y máximo 1 ACU.
- [x] Repositorios ECR creados: `verylike-web`, `verylike-catalog-api` y
  `verylike-forum-api`.
- [x] Rol vinculado a servicio de ECS creado: `AWSServiceRoleForECS`.
- [ ] Crear el clúster ECS y sus recursos de ejecución.
- [ ] Crear secretos de aplicación, servicios ECS y balanceador.
- [ ] Configurar GitHub Actions y lanzar el primer despliegue.

## 1. Crear el clúster ECS

1. Abre **Amazon ECS → Clusters → Create cluster**.
2. Nombre: `verylike-cluster`.
3. Selecciona **Fargate only**.
4. En Container Insights, selecciona **Turned off** para esta fase.
5. Deja vacío el namespace por ahora y crea el clúster.

Si aparece el error *Unable to assume the service linked role*, en IAM crea el
rol vinculado al servicio de ECS o ejecuta en CloudShell:

```bash
aws iam create-service-linked-role --aws-service-name ecs.amazonaws.com
```

## 2. Crear roles para las tareas

En IAM crea estos dos roles:

| Rol | Servicio de confianza | Permisos necesarios |
| --- | --- | --- |
| `verylike-ecs-execution-role` | Elastic Container Service → ECS Task | `AmazonECSTaskExecutionRolePolicy`; permiso de lectura para los secretos de VeryLike. |
| `verylike-ecs-task-role` | Elastic Container Service → ECS Task | Sin permisos extra inicialmente. |

Anota los ARN de ambos roles. El workflow los necesita como variables de
GitHub. La execution role descarga las imágenes, manda logs a CloudWatch y lee
los secretos antes de iniciar cada contenedor.

## 3. Crear secretos de aplicación

Aurora ya creó un secreto de credenciales maestras. No publiques su valor ni lo
subas al repositorio.

El código usa una cadena Npgsql completa, así que crea dos secretos en
**AWS Secrets Manager → Store a new secret → Other type of secret**:

### `verylike/database`

Tipo: texto JSON. Sustituye los valores por el *writer endpoint* de Aurora y
las credenciales del secreto maestro de RDS:

```json
{
  "connectionString": "Host=WRITER_ENDPOINT;Port=5432;Database=verylike;Username=verylikeadmin;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
}
```

### `verylike/tmdb`

```json
{ "apiKey": "TU_CLAVE_NUEVA_DE_TMDB" }
```

Da a `verylike-ecs-execution-role` permiso `secretsmanager:GetSecretValue`
solo para estos secretos.

> Crear secretos tiene un costo pequeño mensual. Si el costo es crítico, se
> puede modificar la aplicación para consumir directamente los campos del
> secreto administrado por RDS; no escribas contraseñas en archivos o variables
> de GitHub.

## 4. Red y seguridad

1. Crea un security group de tareas: `verylike-ecs-sg`.
2. En `verylike-aurora-sg`, agrega una regla de entrada:
   - Tipo: PostgreSQL
   - Puerto: `5432`
   - Origen: security group `verylike-ecs-sg`
3. Mantén Aurora sin acceso público.
4. Crea un Application Load Balancer público para la web y un security group
   `verylike-alb-sg` que permita HTTP 80 desde Internet.
5. En `verylike-ecs-sg`, permite entrada TCP 8080 **solo** desde
   `verylike-alb-sg`.
6. Agrega además una regla de entrada TCP 8080 con origen
   `verylike-ecs-sg` (el propio grupo). Esto permite que Web llame a las APIs,
   sin permitir tráfico externo directo.
7. Para esta primera implementación, usa las subredes de la VPC predeterminada
   y habilita **Assign public IP** en las tareas Fargate. Es necesario para que
   descarguen imágenes de ECR y lean secretos sin instalar un NAT Gateway. La
   base continúa privada, y las reglas anteriores evitan que las APIs acepten
   tráfico externo.
8. Crea un namespace privado de Cloud Map llamado `verylike.local` en la VPC
   predeterminada. Los servicios `catalog-api` y `forum-api` se registrarán
   ahí; no necesitan un balanceador público.

## 5. Logs y definiciones de tarea

1. Crea el log group de CloudWatch `/ecs/verylike`.
2. Abre los archivos de `.aws/ecs/` y conserva los nombres de contenedor:
   `web`, `catalog-api` y `forum-api`.
3. El workflow sustituye automáticamente los marcadores de rol y región de las
   definiciones de tarea. No pegues secretos en estos JSON.
4. Al crear servicios ECS, usa Fargate, la VPC predeterminada, sus subredes
   predeterminadas, **Assign public IP: Enabled** y security group
   `verylike-ecs-sg`.
5. Registra Catalog y Forum en Cloud Map con los nombres exactamente
   `catalog-api` y `forum-api`.
6. Conecta únicamente el servicio Web al target group del ALB. El health check
   debe apuntar a `/health` y al puerto `8080`.

## 6. Primera migración de base de datos

Por seguridad, las migraciones no se ejecutan en cada arranque normal.

1. En la primera task definition de `web`, cambia temporalmente:

```json
{ "name": "Database__ApplyMigrations", "value": "true" }
```

2. Despliega solo una tarea web y revisa CloudWatch Logs hasta que inicie sin
   errores.
3. Devuelve ese valor a `false` y despliega de nuevo. En adelante las
   migraciones se aplican de forma explícita, no accidentalmente.

## 7. Configurar GitHub Actions

Crea un proveedor OIDC de GitHub en IAM y un rol para el repositorio
`Venus-Semino/VeryLike`, limitado a la rama `pipeline-ci`. El rol necesita
acceso de escritura a los tres repositorios ECR y permisos para registrar task
definitions y actualizar los servicios ECS.

En **GitHub → Settings → Secrets and variables → Actions** configura:

| Tipo | Nombre | Valor |
| --- | --- | --- |
| Secret | `AWS_ROLE_TO_ASSUME` | ARN del rol OIDC de GitHub Actions. |
| Variable | `AWS_REGION` | `us-east-2` |
| Variable | `AWS_ACCOUNT_ID` | ID de tu cuenta AWS. |
| Variable | `ECS_CLUSTER` | `verylike-cluster` |
| Variable | `ECS_SERVICE_WEB` | Nombre del servicio Web de ECS. |
| Variable | `ECS_SERVICE_CATALOG` | Nombre del servicio Catalog de ECS. |
| Variable | `ECS_SERVICE_FORUM` | Nombre del servicio Forum de ECS. |
| Variable | `ECS_EXECUTION_ROLE_ARN` | ARN de `verylike-ecs-execution-role`. |
| Variable | `ECS_TASK_ROLE_ARN` | ARN de `verylike-ecs-task-role`. |

Después, cada `git push origin pipeline-ci` ejecutará el workflow de despliegue.

## 8. Uso local

No es necesario usar Docker para que la aplicación permanezca disponible: eso
lo hace ECS. Docker local solo sirve para pruebas y no podrá conectarse a la
base privada sin una VPN o túnel autorizado.

Nunca subas `.env`, contraseñas, endpoints privados, ni la clave de TMDB al
repositorio. Si una clave se publicó por accidente, rótala de inmediato.

## Costos y apagado

Aurora con mínimo 0 ACU puede pausar cómputo tras inactividad, pero siguen
existiendo costos de almacenamiento, respaldos y secretos. ECS/Fargate y el
Application Load Balancer también cobran mientras existan tareas o balanceador.
Para dejar de generar costos, escala los servicios ECS a 0, elimina el ALB y,
si ya no necesitas la base, crea un snapshot final y elimina Aurora.
