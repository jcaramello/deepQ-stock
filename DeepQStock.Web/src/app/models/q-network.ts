/**
 * Q Network
 * 
 * @export
 * @class QNetwork
 */
export class QNetwork{

    //Public Properties
    public hiddenLayersCount:number;
    public neuronCountForHiddenLayers: number;
    public activationFcForHiddenLayers: string;
    public activationFcForOuputLayer: string;

    /**
     * Creates an instance of QNetwork.
     * 
     * @memberof QNetwork
     */
    constructor(){

        this.hiddenLayersCount = 4;
        this.neuronCountForHiddenLayers = this.hiddenLayersCount * 4;
        this.activationFcForHiddenLayers = 'tanh'
        this.activationFcForOuputLayer = 'sigmoid'

    }
}