# 需要添加到数据库中的字段

## user:





## rider:





## merchant: 

`dish` 表中缺少 “库存数量” 这一属性✔️
public required int StockQuantity { get; set; }

`order` 表中缺少 “merchantId” 这一属性
[StringLength(450)]
[Column(TypeName = "VARCHAR(450)")]
public required string MerchantId { get; set; }

`dish` 表中可增加 “描述” 这一属性？✔️
[StringLength(65535)]
public string? Description { get; set; } 

`merchant` 表中可增加 “描述” 这一属性？✔️
[StringLength(65535)]
public string? Description { get; set; } 

## admin:

`Application` 表中缺少 “申请材料” 这一属性✔️
[StringLength(65535)]
public string? ApplicationMaterials { get; set; }

`Coupon` 表中缺少 “是否被使用” 这一属性✔️
public required bool IsUsed { get; set; }
