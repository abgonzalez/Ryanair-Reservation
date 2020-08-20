using System.ComponentModel.DataAnnotations;
/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class ReservationRequest
{
    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string email { get; set; }

    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string creditCard { get; set; }

    [Required]
    [System.Xml.Serialization.XmlElementAttribute("flights", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ReservationFlightRequest[] flights { get; set; }

}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class ReservationFlightRequest
{
    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string key { get; set; }

    [Required]
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("passengers", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PassengersReservationDetailsRequest[] passengers { get; set; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PassengersReservationDetailsRequest
{



    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string name { get; set; }


    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int bags { get; set; }


    [Required]
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int seat { get; set; }

}

