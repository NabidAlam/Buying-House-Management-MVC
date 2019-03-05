--GetOrderMasterInfo(30 April,2018)
select * from  ExFactoryMas exFactoryMas
         inner join  ExFactoryDet exFactoryDet on exFactoryMas.Id = exFactoryDet.ExFactoryMasId
         inner join  BuyerOrderMas buyerOrder on exFactoryDet.BuyerOrderMasId = buyerOrder.Id
         where exFactoryMas.SupplierId = 2 and exFactoryMas.BuyerInfoId = 1 and exFactoryMas.ExFactoryDate = '2018-04-24'




--BuyerDetails

select * from  ExFactoryMas exFactoryMas
    inner join  ExFactoryDet exfactoryDet on exFactoryMas.Id = exfactoryDet.ExFactoryMasId
    inner join  BuyerOrderMas buyerOrderMas on exfactoryDet.BuyerOrderMasId = buyerOrderMas.Id
    inner join  buyerOrderDet BuyerOrderDet on buyerOrderMas.Id = buyerOrderDet.BuyerOrderMasId
    inner join  FactoryOrderDet factoryOrderDet on buyerOrderDet.Id = factoryOrderDet.BuyerOrderDetId
    inner join  ShipmentSummDet shipDet on buyerOrderDet.Id = shipDet.BuyerOrderDetId
    left join  DestinationPort dest  on shipDet.DestinationPortId = dest.Id 
    where buyerOrderMas.Id = 17



	--Buyer Order Factory Wise
--select * from  BuyerOrderMas buyerMas
--inner join  BuyerOrderDet buyerDet on buyerMas.Id = buyerDet.BuyerOrderMasId                       
--inner join  FactoryOrderMas factoryMas on  buyerDet.BuyerOrderMasId = factoryMas.BuyerOrderMasId and buyerDet.SupplierId=factoryMas.SupplierId
--inner join  FactoryOrderDet factoryDet on factoryMas.Id =factoryDet.FactoryOrderMasId and buyerDet.Id = factoryDet.BuyerOrderDetId 
--left join  MasterLCInfoDet masterLCDet on buyerMas.Id = masterLCDet.BuyerOrderMasId
--left join  MasterLCInfoMas masterLCMas on masterLCDet.MasterLCInfoMasId = masterLCMas.Id
