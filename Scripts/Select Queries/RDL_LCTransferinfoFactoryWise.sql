select * from LCTransferDet transferDet
inner join LCTransferMas transferMas on transferDet.LCTransferMasId = transferMas.Id
inner join FactoryOrderMas factMas on transferDet.FactoryOrderMasId = factMas.Id
where factMas.SupplierId=2
