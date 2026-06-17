using System.Collections.Generic;
using System.Xml.Serialization;

namespace Univision.Exchange
{
  [XmlRoot("response")]
  public class ExchangeXMLModel
  {
    [XmlElement("header")]
    public ExchangeCustomHeaderModel header { get; set; }

    [XmlElement("body")]
    public ExchangeCustomBodyModel body { get; set; }
  }

  public class ExchangeCustomHeaderModel
  {

    [XmlElement("resultCode")]
    public int resultCode { get; set; }

    [XmlElement("resultMsg")]
    public string resultMsg { get; set; }

  }

  public class ExchangeCustomBodyModel
  {
    [XmlElement("items")]
    public ExchangeCustomItemsModel items { get; set; }
  }

  public class ExchangeCustomItemsModel
  {
    [XmlElement("item")]
    public List<ExchangeCustomItemModel> item { get; set; }
  }

  public class ExchangeCustomItemModel
  {
    [XmlElement("aplyBgnDt")]
    public string aplyBgnDt { get; set; }

    [XmlElement("cntySgn")]
    public string cntySgn { get; set; }

    [XmlElement("currSgn")]
    public string currSgn { get; set; }

    [XmlElement("fxrt")]
    public double fxrt { get; set; }

    [XmlElement("imexTp")]
    public int imexTp { get; set; }

    [XmlElement("mtryUtNm")]
    public string mtryUtNm { get; set; }

  }

}