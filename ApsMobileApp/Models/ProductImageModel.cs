using ShareModels;
using ShareModels.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace ApsMobileApp.Models;

public class ProductImageModel  : ProductImage
{
    public ProductImageModel() { }
    public ProductImageModel(ProductImage item)
    {
        this.Id = item.Id;
        this.Buffer = item.Buffer;
        this.FileName = item.FileName;
        this.FileType = item.FileType;
        this.ProductId = item.ProductId;
        this.Thumb = item.Thumb;
    }

    public Uri PhotoView
    {
        get
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                var uri= new   Uri($"{Helper.Url}/images/products/{FileName}");
                    return uri;
            }
            else
                return new Uri($"noimage.png");
        }
    }


    public Uri ThumbView
    {
        get
        {
            if (!string.IsNullOrEmpty(Thumb))
            {
                return new Uri($"{Helper.Url}/images/thumbs/{Thumb}");
            }
            else
                return new Uri($"noimage.png");
        }
    }

}
