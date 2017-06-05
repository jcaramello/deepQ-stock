![DeepQ Stock](https://raw.githubusercontent.com/jcaramello/deepQ-stock/master/DeepQStock.Web/src/img/logo.png)

Inspirado en [DeepMind](https://deepmind.com/research/dqn/),
el proyecto busca investigar la aplicabilidad y efectividad de estas ideas, 
en el desarrollo de agentes autónomos que combinando **Deep Neural Networks** con
**Reinforcement Learning**, aprendan a invertir en activos financieros.

## El Proyecto

El proyecto consta de 2 aplicaciones, una api auto-hosteada implementada con ASP.NET 5 y Web Api 2,
Y una aplicacion web implementada con Angular 4, Typescript, Bootrstrap 4 y SignalR

## Configuracion

Para poder ejecutar la applicacion es necesario configuar localmente:

* Redis, sera usado como base de datos por la api
* La Api, usada por la aplicacion web.
* La applicacion web

### Redis

Windows: Instalar Redis desde [Microsoft Open Tech](https://msopentech.com/blog/2015/03/03/redis-windows-2-8-19-released/)

*port*: 6379

### Servidor 

La api, esta implementada con Owin, por lo cual podra auto hostearse, sin la neceisdad de instalar un servidor web.
Para levantar el servidor web:

```
    cd DeepQStock.Server/bin
    dqs-server
```

### Aplicacion Web

Descargar e Instalar node js version 6.10+
Una vez instalado habra que instlar los siguientes paquetes en forma global

```
    npm install typescript -g    
    npm install bower -g
```

Una vez instalado typescript, es necesario instalar las dependencias del proyecto:
```
    cd DeepQStock.Web
    npm install
    bower install
```
Finalmente podremos levantar el servidor web usando angular cli
```
    ng serve
```
La aplicacion web estara accesible en 

```
   http://localhost:4200/
```

