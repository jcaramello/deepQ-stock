<p align="center">
  <img src="https://raw.githubusercontent.com/jcaramello/deepQ-stock/master/DeepQStock.Web/src/img/logo.png" alt="DeepQ Stock"/>
</p>

Inspirado en [DeepMind](https://deepmind.com/research/dqn/), el proyecto busca investigar la aplicabilidad y efectividad de **Deep Reinforcement Learning**, 
en el desarrollo de agentes autónomos que combinando **Deep Neural Networks** con
**Reinforcement Learning**, aprendan a invertir en activos financieros.

# Referencias
 
 * [Reinforcement Learning: An Introduction - Richard S. Sutton and Andrew G. Barto](http://people.inf.elte.hu/lorincz/Files/RL_2006/SuttonBook.pdf)
 * [Bekerly UC - Curse: Deep Reinforcement Learning](http://rll.berkeley.edu/deeprlcourse/)
 * [Stanford - Curse: Convolutional Neural Networks for Visual Recognition](http://karpathy.github.io/2016/05/31/rl/)   
 * [Neural Networks and Deep Learning](http://neuralnetworksanddeeplearning.com/)
 * [An Investigation into the Use of Reinforcement Learning Techniques within the Algorithmic Trading Domain](http://www.doc.ic.ac.uk/teaching/distinguished-projects/2015/j.cumming.pdf)
 * [Algorithm Trading using Q-Learning and Recurrent Reinforcement Learning](http://cs229.stanford.edu/proj2009/LvDuZhai.pdf)
 * [Demystifying deep reinforcement learning](http://neuro.cs.ut.ee/demystifying-deep-reinforcement-learning/)
 * [Deep Q-Learning for Stock Trading](http://hallvardnydal.github.io/2016/03/12/deep_q/)
 
## El Proyecto

El proyecto consta de 2 aplicaciones, una api sefl-hostead implementada con ASP.NET 5, SignalR y Encog3
Y una aplicacion web implementada con Angular 4 y Typescript

## Configuracion

Para poder ejecutar la applicacion en forma local, es necesario configurar:

* Redis, sera usado como base de datos por la api
* Un Servidor donde se ejecutaran los agentes y que brindara actualizaciones en tiempo real del estado de los mismos.
* Una aplicacion web, donde se podran visualizar el aprendizaje alcanzado por los agentes creados asi como tambien diferentes estadisticas.

### Redis

Instalar Redis desde [Microsoft Open Tech](https://msopentech.com/blog/2015/03/03/redis-windows-2-8-19-released/)

*port*: 6379

### Servidor 

El Servidor consta de una api self-hosted, implementada con Owin y signalR.
<br>
Para levantar la api, ejecutar el archivo **dqs-server.exe** que se encuentra en DeepQStock.Server/bin:

```
    cd DeepQStock.Server/bin
    dqs-server
```

### Aplicacion Web

Para ejecutar la applicacion web sera necesario descargar e Instalar [node.js](https://nodejs.org/es/download/), version 6.10+
<br>
Una vez instalado habra que instalar los siguientes paquetes en forma global

```
    npm install typescript -g    
    npm install bower -g
```

Luego será necesario instalar las dependencias del proyecto:
```
    cd DeepQStock.Web
    npm install
    bower install
```
Finalmente podremos levantar el servidor web usando angular-cli
```
    ng serve
```
La aplicacion web estara accesible en 

```
   http://localhost:4200/
```