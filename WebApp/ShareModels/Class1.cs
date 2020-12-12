
//SELECT
//  `Product`.`Id`,
//  `Product`.`CategoryId`,
//  `Product`.`Merk`,
//  `Product`.`Name`,
//  `Product`.`CodeName`,
//  `Product`.`CodeArticle`,
//  `Product`.`Description`,
//  `Product`.`Size`,
//	 (SELECT
//          SUM(PembelianItem.Amount * Unit.Amount)
//            FROM
//  			`Pembelian`
//  		LEFT JOIN `PembelianItem` ON `Pembelian`.`Id` = `PembelianItem`.`PembelianId`
//  		LEFT JOIN `Unit` ON `PembelianItem`.`UnitId` = `Unit`.`Id` where product.id = PembelianItem.productid and `Pembelian`.`Status` > 0) as pembelian,
//	(
//        SELECT
//  SUM(Penjualanitem.Amount * Unit.Amount)
//FROM
//  `Penjualan`
//  LEFT JOIN `Penjualanitem` ON `Penjualan`.`Id` = `Penjualanitem`.`PenjualanId`
//  LEFT JOIN `Unit` ON `Penjualanitem`.`UnitId` = `Unit`.`Id`
//where	 Penjualan.activity > 0 and Product.id = Penjualanitem.productid  ) as penjualan


//FROM
//  `Product`
  
//GROUP BY
//  `Product`.`Id`,
//  `Product`.`CategoryId`,
//  `Product`.`Merk`,
//  `Product`.`Name`,
//  `Product`.`CodeName`,
//  `Product`.`CodeArticle`,
//  `Product`.`Description`,
//  `Product`.`Size`;