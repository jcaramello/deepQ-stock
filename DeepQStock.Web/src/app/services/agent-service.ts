
import { Injectable, EventEmitter } from '@angular/core';
import { Agent } from '../models/agent';
import { OnDayCompletedArgs } from '../models/on-day-completed-args';
import { BaseService } from './base-service';

/**
 * Agent Service
 * 
 * @export
 * @class AgentService
 */
@Injectable()
export class AgentService extends BaseService {

    // public events
    public onCreatedAgent: EventEmitter<Agent>;
    public onDayCompleted: EventEmitter<OnDayCompletedArgs>;

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('agentHub');

        this.onCreatedAgent = new EventEmitter<Agent>();
        this.onDayCompleted = new EventEmitter<OnDayCompletedArgs>();
    }

    /**
     * Get all agents
     */
    public getAll() {
        return this.execute('getAll');
    }

    /**
     * Get an agent by id 
     * @param id
     */
    public getById(id: number) {
        return this.execute('getById', id)
    }

    /**
     * Start the simulation of a particular agent      
     * @memberof AgentService
     */
    public start(id: number) {
        this.execute('start', id);
    }

    /**
     * 
     * @param args Trigger when the agent complete the simulation of a day
     */
    public dayCompleted(args: OnDayCompletedArgs) {
        this.onDayCompleted.emit(args);
    }
}