
import { Injectable, EventEmitter } from '@angular/core';
import { Agent } from '../models/agent';
import { BaseService } from './base.service';

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

    /**
     * Creates an instance of AgentService.
     * 
     * @memberof AgentService
     */
    constructor() {
        super('agentHub');

        this.onCreatedAgent = new EventEmitter<Agent>();       
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
    public getById(id: number){
        return this.execute('getById', id)
    }
}