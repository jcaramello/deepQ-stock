import { QNetwork } from './q-network';
import { OnDayComplete } from './on-day-complete';
import { AgentStatus } from './enums';

/**
 * Agent
 * 
 * @export
 * @class Agent
 */
export class Agent {

    // Public properties
    public id: number;
    public name: string;
    public eGreedyProbability: number;
    public inOutStrategy: number;
    public miniBatchSize: number;
    public discountFactor: number;
    public memoryReplaySize: number;
    public qNetwork: QNetwork
    public qNetworkId: number;
    public stockExchangeParametersId: number;
    public decisions: OnDayComplete[];
    public status: AgentStatus;

    /**
     * Creates an instance of Agent.
     * 
     * @memberof Agent
     */
    constructor() {
        this.eGreedyProbability = 0.1;
        this.inOutStrategy = 0.33;
        this.miniBatchSize = 50;
        this.discountFactor = 0.8;
        this.memoryReplaySize = 500;
        this.qNetwork = new QNetwork();
    }
}