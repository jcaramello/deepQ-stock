
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
    public onDayCompleted: EventEmitter<OnDayCompletedArgs>;

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('agentHub');
        
        this.onDayCompleted = new EventEmitter<OnDayCompletedArgs>();

        this.init();
    }

    /**
     * Get all agents
     */
    public getAll(): Promise<Agent[]> {
        return this.execute('getAll');
    }

    /**
     * Get an agent by id 
     * @param id
     */
    public getById(id: number): Promise<Agent> {
        return this.execute('getById', id)
    }

    /**
     * Save an agent
     * @param agent 
     */
    public save(agent: Agent){
        return this.execute('save', agent);
    }

    /**
     * Start the simulation of a particular agent      
     * @memberof AgentService
     */
    public start(id: number): void {
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