select * from [dbo].[OnDayCompletes] where Agent_Id = 1
select * from [dbo].[DeepRLAgentParameters]
select * from [dbo].[StockExchangeParameters]

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
select * from [dbo].[SimulationResults]  where AgentId = 1
select * from [dbo].[Experiences] where AgentId = 1

BULK INSERT [SimulationResults]
    FROM 'C:\Users\jcaramello\projects\deepQ-stock\simulationresults.csv'
    WITH
    (
		FIRSTROW = 0,
		FIELDTERMINATOR = ',',  --CSV field delimiter
		ROWTERMINATOR = '\n',   --Use to shift the control to next row
		ERRORFILE = 'C:\Users\jcaramello\projects\deepQ-stock\ErrorRows.csv',
		TABLOCK
    )