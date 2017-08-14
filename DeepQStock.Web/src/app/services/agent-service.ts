
import { Injectable, EventEmitter } from '@angular/core';
import { Agent } from '../models/agent';
import { OnDayComplete } from '../models/on-day-complete';
import { OnSimulationComplete } from '../models/on-simulation-complete';
import { OnTrainingEpochComplete } from '../models/on-training-epoch-complete';
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
    public onDayCompleted: EventEmitter<OnDayComplete>;
    public onSimulationCompleted: EventEmitter<OnSimulationComplete>;
    public onTrainingEpochCompleted: EventEmitter<OnTrainingEpochComplete>;
    public onCreatedAgent: EventEmitter<Agent>;

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('agentHub');

        this.onDayCompleted = new EventEmitter<OnDayComplete>();
        this.onSimulationCompleted = new EventEmitter<OnSimulationComplete>();
        this.onTrainingEpochCompleted = new EventEmitter<OnTrainingEpochComplete>();
        this.onCreatedAgent = new EventEmitter<Agent>();

        this.proxy.on('onCreatedAgent', a => this.onCreatedAgent.emit(a));
        this.proxy.on('onDayComplete', a => this.onDayCompleted.emit(a));
        this.proxy.on('onSimulationComplete', a => this.onSimulationCompleted.emit(a));
        this.proxy.on('onTrainingEpochCompleted', a => this.onTrainingEpochCompleted.emit(a));

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
        return this.execute('getById', id);
    }

    /**
     * Save an agent
     * @param agent 
     */
    public save(agent: Agent) {
        return this.execute('save', agent);
    }

    public getDecisions(id: number) {
        return this.execute('getDecisions', id);
    }

    /**
     * Start the simulation of a particular agent      
     * @memberof AgentService
     */
    public start(id: number): void {
        this.execute('start', id);
    }

    /**
     * Subscribe to agent events 
     * @param {number} id 
     * @memberof AgentService
     */
    public subscribe(id:number){
        this.execute('subscribe', id);
    }

    /**
     * Stop the simulation of a particular agent      
     * @memberof AgentService
     */
    public pause(id: number): void {
        this.execute('pause', id);
    }
        
    /**
     * Remove an agent 
     * @param {number} id 
     * @memberof AgentService
     */
    public remove(id: number){
        return this.execute('remove', id);
    }
}