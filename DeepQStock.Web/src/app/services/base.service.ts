
import { environment } from '../../environments/environment';

/**
 * Base Service
 * 
 * @export
 * @class BaseService
 */
export class BaseService {

    // Private fields
    protected connection: SignalR.Hub.Connection;
    protected proxy: SignalR.Hub.Proxy;
    protected onConnected: Promise<any>;


    /**
     * Creates an instance of BaseService.
     * @param {string} hubName 
     * 
     * @memberof BaseService
     */
    constructor(hubName: string) {

        this.connection = $.hubConnection(environment.signalrUrl);
        this.proxy = this.connection.createHubProxy(hubName);
        this.onConnected = Promise.resolve(this.connection.start().then(() =>  console.log(`connnection established with ${hubName}`)));
    }

    /**
     * execute a method in the server
     * 
     * @protected
     * @param {string} method 
     * @param {...any[]} args 
     * @returns 
     * 
     * @memberof BaseService
     */
    protected execute(method: string, ...args: any[]) {
        return this.onConnected.then(() => Promise.resolve(this.proxy.invoke('getAll')));
    }
}