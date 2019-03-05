--select supplier.Id,supplier.Name,buyer.Name,buyerMas.Id as 'Buyer Order Mas',buyerMas.OrderRefNo,
--buyerDet.Quantity, buyerDet.Quantity * buyerDet.UnitPrice as 'RDL Value',
--factoryDet.FOBUnitPrice * buyerDet.Quantity as 'Factory Value',
--buyerMas.OrderDate
select supplier.Id,buyer.Id,buyerMas.Id as 'Buyer Order Mas',
sum(buyerDet.Quantity) as 'Order Qty', sum(buyerDet.Quantity * buyerDet.UnitPrice) as 'RDL Value',
sum(factoryDet.FOBUnitPrice * buyerDet.Quantity) as 'Factory Value',
buyerMas.OrderDate,masterLCMas.LatestShipmentDate as 'ExFactory Date'
from [dbo].[BuyerOrderMas] buyerMas
inner join [dbo].[BuyerOrderDet] buyerDet on buyerMas.Id = buyerDet.BuyerOrderMasId
inner join [dbo].[FactoryOrderMas] factoryMas on buyerMas.Id = factoryMas.BuyerOrderMasId
inner join [dbo].[FactoryOrderDet] factoryDet on factoryMas.Id = factoryDet.FactoryOrderMasId
left join [dbo].[MasterLCInfoDet] masterLCDet on buyerMas.Id = masterLCDet.BuyerOrderMasId
left join [dbo].[MasterLCInfoMas] masterLCMas on masterLCDet.MasterLCInfoMasId = masterLCMas.Id
inner join [dbo].[BuyerInfo] buyer on buyerMas.BuyerInfoId = buyer.Id
inner join [dbo].[Supplier] supplier on factoryMas.SupplierId = supplier.Id
where factoryMas.SupplierId = 2
group by supplier.Id,buyer.Id,buyerMas.Id,buyerMas.OrderDate,masterLCMas.LatestShipmentDate

select * from [dbo].[BuyerOrderMas] buyerMas
inner join [dbo].[BuyerOrderDet] buyerDet on buyerMas.Id = buyerDet.BuyerOrderMasId
inner join [dbo].[FactoryOrderMas] factoryMas on buyerMas.Id = factoryMas.BuyerOrderMasId

select * from [dbo].[FactoryOrderMas] factoryMas 
inner join [dbo].[FactoryOrderDet] factoryDet on factoryMas.Id = factoryDet.FactoryOrderMasId


--RDL value and Factory Value
select buyDet.Quantity * factDet.FOBUnitPrice from  BuyerOrderMas buyMas
   inner join  BuyerOrderDet buyDet on buyMas.Id = buyDet.BuyerOrderMasId
   inner join  FactoryOrderMas factMas on buyDet.BuyerOrderMasId=factMas.BuyerOrderMasId and buyDet.SupplierId = factMas.SupplierId 
   inner join  FactoryOrderDet factDet on factMas.Id = factDet.FactoryOrderMasId
   where buyMas.Id = 24 and factMas.SupplierId=1


   select * from  BuyerOrderMas buyMas
   inner join  BuyerOrderDet buyDet on buyMas.Id = buyDet.BuyerOrderMasId
   inner join  FactoryOrderMas factMas on buyDet.BuyerOrderMasId=factMas.BuyerOrderMasId and buyDet.SupplierId = factMas.SupplierId 
   inner join  FactoryOrderDet factDet on factMas.Id = factDet.FactoryOrderMasId and factDet.BuyerOrderDetId = buyDet.Id
   where buyMas.Id = 33 and buyDet.SupplierId=1