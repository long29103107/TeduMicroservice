﻿using Shared.Enums.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Inventory
{
    public class PurchaseProductDto
    {
        public EDocumentType DocumentType => EDocumentType.Purchase;
        public string? ItemNo { get; set; }
        public string? DocumentNo { get; set; }
        public string?  ExternalDocumentNo { get; set; }
        public int Quantity { get; set; }
        public void SetItemNo(string itemNo)
        {
            ItemNo = itemNo;
        }
    }
}
