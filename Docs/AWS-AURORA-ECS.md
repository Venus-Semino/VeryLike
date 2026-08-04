# Despliegue en AWS: Aurora PostgreSQL Serverless v2 + ECS/Fargate

Esta configuración no utiliza una base de datos local ni App Runner. Los tres
contenedores se ejecutan en ECS/Fargate y Aurora permanece privada en la VPC.

## 1. Red y base de datos

1. Crea una VPC con subredes privadas en al menos dos zonas de disponibilidad.
2. Crea un clúster **Aurora PostgreSQL Compatible, Serverless v2** y una base
   inicial llamada `verylike`. No habilites acceso público.
3. Crea un security group para las tareas ECS y otro para Aurora. En el de
   Aurora permite TCP 5432 **solo** desde el security group de ECS.
4. Crea un namespace privado de Cloud Map: `verylike.local`; registra los
   servicios `catalog-api` y `forum-api`. El servicio web debe ir detrás de un
   Application Load Balancer público con comprobación `GET /health`.

## 2. Secretos

En AWS Secrets Manager crea secretos JSON:

```json
// verylike/database
{ "connectionString": "Host=CLUSTER_ENDPOINT;Port=5432;Database=verylike;Username=verylike_app;Password=CONTRASENA;SSL Mode=Require;Trust Server Certificate=true" }

// verylike/tmdb
{ "apiKey": "TU_CLAVE_NUEVA_DE_TMDB" }
```

La tarea ECS debe tener una *execution role* con `secretsmanager:GetSecretValue`
para esos secretos y permisos de escritura en CloudWatch Logs. La *task role*
puede empezar sin permisos adicionales.

## 3. ECS y ECR

1. Crea los repositorios ECR: `verylike-web`, `verylike-catalog-api` y
   `verylike-forum-api`.
2. Crea el cluster ECS y tres servicios Fargate con las task definitions de
   `.aws/ecs/`. Configura Cloud Map para las dos APIs y el ALB para `web`.
3. Crea el log group `/ecs/verylike`.
4. Para la primera inicialización, actualiza temporalmente la task definition
   de `web` para usar `Database__ApplyMigrations=true`, escala web a una sola
   tarea y espera a que termine. Después vuelve el valor a `false` y despliega
   normalmente. Así ningún despliegue cotidiano ejecuta DDL.

## 4. GitHub Actions

Crea el proveedor OIDC de GitHub y un rol limitado a
`repo:Venus-Semino/VeryLike:ref:refs/heads/pipeline-ci`. El rol requiere acceso
a ECR y permisos para registrar task definitions y actualizar los tres
servicios ECS.

En **GitHub → Settings → Secrets and variables → Actions**, configura:

| Tipo | Nombre |
| --- | --- |
| Secret | `AWS_ROLE_TO_ASSUME` |
| Variable | `AWS_REGION`, `AWS_ACCOUNT_ID`, `ECS_CLUSTER` |
| Variable | `ECS_SERVICE_WEB`, `ECS_SERVICE_CATALOG`, `ECS_SERVICE_FORUM` |
| Variable | `ECS_EXECUTION_ROLE_ARN`, `ECS_TASK_ROLE_ARN` |

Cada `push` a `pipeline-ci` compila, construye las tres imágenes, las publica
en ECR con el SHA del commit y registra/despliega task definitions nuevas.
