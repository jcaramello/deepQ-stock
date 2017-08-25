select * from [dbo].[OnDayCompletes] where Agent_Id = 1
select * from [dbo].[DeepRLAgentParameters]
select * from [dbo].[StockExchangeParameters]

UPDATE DeepRLAgentParameters SET Status= 4 WHERE ID =3

select * from [dbo].[AverageTrueRanges] where StockExchangeId = 1
select * from [dbo].[BollingerBandsPercentBs] where StockExchangeId = 1
select * from [dbo].[DMIs] where StockExchangeId = 1
select * from [dbo].[MACDs] where StockExchangeId = 1
select * from [dbo].[RSIs]  where StockExchangeId = 1
select * from [dbo].[SimpleMovingAverages] where StockExchangeId = 1

select * from [dbo].[States] where StockExchangeId = 1
select * from [dbo].[Periods] where StockExchangeId = 1
select * from [dbo].[IndicatorValues]
select * from StatePeriods

select * from [dbo].[DeepRLAgentParameters]
select * from [dbo].[SimulationResults]  

select * from [dbo].[Experiences] where AgentId = 1
SELECT * FROM [HangFire].[JobQueue]
--update DeepRLAgentParameters set eGreedyProbability = 0.3 where id = 4


