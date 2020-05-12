﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLFeedPull.NetCore
{
    /**********************************************************************\
     * AUTO-GENERATED BY CLOVERLEAF -- DO NOT MODIFY
     * This code may be overwritten by fresh copy-paste from the source:
     * https://api.experientdata.com/entityschema?fmt=cs
     * If you need to override something, use a partial class elsewhere.
    \**********************************************************************/

    public partial class SchemaGenAttribute : Attribute { }
    [SchemaGen]
    public partial class Attachment : SchemaEntity
    {
        [SchemaGen]
        public string ChunkFileKey { get; set; }
        [SchemaGen]
        public long FileLength { get; set; }
        [SchemaGen]
        public string FileName { get; set; }
        [SchemaGen]
        public bool IsDirectory { get; set; }
        [SchemaGen]
        public string MimeType { get; set; }
        [SchemaGen]
        public string Purpose { get; set; }
        [SchemaGen]
        public string SourceURL { get; set; }
    }
    [SchemaGen]
    public partial class Booth : SchemaEntityRoot
    {
        private BoothPerson[] _BoothPersons;
        [SchemaGen]
        public BoothPerson[] BoothPersons { get { return _BoothPersons ?? new BoothPerson[0]; } set { _BoothPersons = value; } }
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public bool IsMarking { get; set; }
        [SchemaGen]
        public bool IsNonExhibitingSpace { get; set; }
        [SchemaGen]
        public string Name { get; set; }
        [SchemaGen]
        public string Size { get; set; }
    }
    [SchemaGen]
    public partial class BoothPerson : SchemaEntity
    {
        [SchemaGen]
        public string PersonID { get; set; }
    }
    [SchemaGen]
    public partial class Category : SchemaEntity
    {
        [SchemaGen]
        public string Description { get; set; }
        private string[] _Path;
        [SchemaGen]
        public string[] Path { get { return _Path ?? new string[0]; } set { _Path = value; } }
    }
    [SchemaGen]
    public partial class Company : SchemaEntityRoot
    {
        [SchemaGen]
        public string Address { get; set; }
        [SchemaGen]
        public string Address2 { get; set; }
        [SchemaGen]
        public string Address3 { get; set; }
        [SchemaGen]
        public string AddressFull { get; set; }
        private string[] _AltNames;
        [SchemaGen]
        public string[] AltNames { get { return _AltNames ?? new string[0]; } set { _AltNames = value; } }
        private Attachment[] _Attachments;
        [SchemaGen]
        public Attachment[] Attachments { get { return _Attachments ?? new Attachment[0]; } set { _Attachments = value; } }
        [SchemaGen]
        public string Bio { get; set; }
        private Category[] _Categories;
        [SchemaGen]
        public Category[] Categories { get { return _Categories ?? new Category[0]; } set { _Categories = value; } }
        [SchemaGen]
        public string City { get; set; }
        [SchemaGen]
        public string Company1 { get; set; }
        [SchemaGen]
        public string Company2 { get; set; }
        private CompanyBooth[] _CompanyBooths;
        [SchemaGen]
        public CompanyBooth[] CompanyBooths { get { return _CompanyBooths ?? new CompanyBooth[0]; } set { _CompanyBooths = value; } }
        private CompanyProduct[] _CompanyProducts;
        [SchemaGen]
        public CompanyProduct[] CompanyProducts { get { return _CompanyProducts ?? new CompanyProduct[0]; } set { _CompanyProducts = value; } }
        private CompanyPurchase[] _CompanyPurchases;
        [SchemaGen]
        public CompanyPurchase[] CompanyPurchases { get { return _CompanyPurchases ?? new CompanyPurchase[0]; } set { _CompanyPurchases = value; } }
        [SchemaGen]
        public string Country { get; set; }
        [SchemaGen]
        public string Email { get; set; }
        [SchemaGen]
        public string Email2 { get; set; }
        [SchemaGen]
        public string Facebook { get; set; }
        [SchemaGen]
        public string Fax { get; set; }
        [SchemaGen]
        public bool IsExhibitor { get; set; }
        [SchemaGen]
        public bool IsMember { get; set; }
        [SchemaGen]
        public bool IsSponsor { get; set; }
        [SchemaGen]
        public string Keyer { get; set; }
        [SchemaGen]
        public decimal? Latitude { get; set; }
        [SchemaGen]
        public string LinkedIn { get; set; }
        [SchemaGen]
        public decimal? Longitude { get; set; }
        [SchemaGen]
        public string MemberNumber { get; set; }
        [SchemaGen]
        public string MemberType { get; set; }
        [SchemaGen]
        public string Mobile { get; set; }
        [SchemaGen]
        public string Phone { get; set; }
        [SchemaGen]
        public string Phone2 { get; set; }
        [SchemaGen]
        public string PostalCode { get; set; }
        [SchemaGen]
        public string State { get; set; }
        [SchemaGen]
        public string Twitter { get; set; }
        [SchemaGen]
        public string Website { get; set; }
    }
    [SchemaGen]
    public partial class CompanyBooth : SchemaEntity
    {
        [SchemaGen]
        public string BoothID { get; set; }
    }
    [SchemaGen]
    public partial class CompanyProduct : SchemaEntity
    {
        [SchemaGen]
        public string ProductID { get; set; }
    }
    [SchemaGen]
    public partial class CompanyPurchase : SchemaEntity
    {
        [SchemaGen]
        public DateTime? CancelDate { get; set; }
        [SchemaGen]
        public string CancelType { get; set; }
        [SchemaGen]
        public string ProductCode { get; set; }
        [SchemaGen]
        public string ProductID { get; set; }
        [SchemaGen]
        public DateTime? PurchaseDate { get; set; }
        [SchemaGen]
        public decimal? Quantity { get; set; }
        [SchemaGen]
        public decimal? TotalFees { get; set; }
        [SchemaGen]
        public decimal? TotalPaid { get; set; }
    }
    [SchemaGen]
    public partial class CompoSource : SchemaBase
    {
        [SchemaGen]
        public ObjectId AdapterID { get; set; }
        [SchemaGen]
        public string AdapterType { get; set; }
        [SchemaGen]
        public string HashID { get; set; }
        [SchemaGen]
        public string SourceID { get; set; }
        [SchemaGen]
        public string SubSet { get; set; }
    }
    [SchemaGen]
    public partial class Facility : SchemaEntityRoot
    {
        [SchemaGen]
        public string Address { get; set; }
        [SchemaGen]
        public string Address2 { get; set; }
        [SchemaGen]
        public string Address3 { get; set; }
        [SchemaGen]
        public string AddressFull { get; set; }
        private Attachment[] _Attachments;
        [SchemaGen]
        public Attachment[] Attachments { get { return _Attachments ?? new Attachment[0]; } set { _Attachments = value; } }
        private Category[] _Categories;
        [SchemaGen]
        public Category[] Categories { get { return _Categories ?? new Category[0]; } set { _Categories = value; } }
        [SchemaGen]
        public string City { get; set; }
        [SchemaGen]
        public string Country { get; set; }
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public string Email { get; set; }
        [SchemaGen]
        public string Email2 { get; set; }
        [SchemaGen]
        public string Facebook { get; set; }
        private FacilityFeature[] _FacilityFeatures;
        [SchemaGen]
        public FacilityFeature[] FacilityFeatures { get { return _FacilityFeatures ?? new FacilityFeature[0]; } set { _FacilityFeatures = value; } }
        [SchemaGen]
        public string Fax { get; set; }
        [SchemaGen]
        public bool IsHotel { get; set; }
        [SchemaGen]
        public bool IsPrimary { get; set; }
        [SchemaGen]
        public decimal? Latitude { get; set; }
        [SchemaGen]
        public string LinkedIn { get; set; }
        [SchemaGen]
        public decimal? Longitude { get; set; }
        [SchemaGen]
        public string Mobile { get; set; }
        [SchemaGen]
        public string Name { get; set; }
        [SchemaGen]
        public string Phone { get; set; }
        [SchemaGen]
        public string Phone2 { get; set; }
        [SchemaGen]
        public string PostalCode { get; set; }
        [SchemaGen]
        public string State { get; set; }
        [SchemaGen]
        public string Twitter { get; set; }
        [SchemaGen]
        public string Website { get; set; }
    }
    [SchemaGen]
    public partial class FacilityFeature : SchemaEntity
    {
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public string Name { get; set; }
        [SchemaGen]
        public string Type { get; set; }
    }
    [SchemaGen]
    public partial class FieldDetail : SchemaEntityRoot
    {
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public int? DisplayOrder { get; set; }
        [SchemaGen]
        public string DisplayType { get; set; }
        private FieldDetailPick[] _FieldDetailPicks;
        [SchemaGen]
        public FieldDetailPick[] FieldDetailPicks { get { return _FieldDetailPicks ?? new FieldDetailPick[0]; } set { _FieldDetailPicks = value; } }
        [SchemaGen]
        public string FieldName { get; set; }
        [SchemaGen]
        public string FormatType { get; set; }
        [SchemaGen]
        public bool IsLookup { get; set; }
        [SchemaGen]
        public bool IsMulti { get; set; }
        [SchemaGen]
        public string RecordType { get; set; }
        [SchemaGen]
        public string SelectType { get; set; }
    }
    [SchemaGen]
    public partial class FieldDetailPick : SchemaEntity
    {
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public int? DisplayOrder { get; set; }
        [SchemaGen]
        public string OtherFieldDetailID { get; set; }
        [SchemaGen]
        public string PickCode { get; set; }
    }
    [SchemaGen]
    public partial class Map : SchemaEntityRoot
    {
        private Attachment[] _Attachments;
        [SchemaGen]
        public Attachment[] Attachments { get { return _Attachments ?? new Attachment[0]; } set { _Attachments = value; } }
        [SchemaGen]
        public MapTransform BoothTransform { get; set; }
        private Category[] _Categories;
        [SchemaGen]
        public Category[] Categories { get { return _Categories ?? new Category[0]; } set { _Categories = value; } }
        [SchemaGen]
        public string Description { get; set; }
        private MapBooth[] _MapBooths;
        [SchemaGen]
        public MapBooth[] MapBooths { get { return _MapBooths ?? new MapBooth[0]; } set { _MapBooths = value; } }
        [SchemaGen]
        public string Name { get; set; }
    }
    [SchemaGen]
    public partial class MapBooth : SchemaEntity
    {
        [SchemaGen]
        public string BoothID { get; set; }
        private MapPoint[] _Polygon;
        [SchemaGen]
        public MapPoint[] Polygon { get { return _Polygon ?? new MapPoint[0]; } set { _Polygon = value; } }
    }
    [SchemaGen]
    public partial class MapPoint : SchemaBase
    {
        [SchemaGen]
        public decimal X { get; set; }
        [SchemaGen]
        public decimal Y { get; set; }
    }
    [SchemaGen]
    public partial class MapTransform : SchemaBase
    {
        [SchemaGen]
        public MapPoint AxisX { get; set; }
        [SchemaGen]
        public MapPoint AxisY { get; set; }
        [SchemaGen]
        public MapPoint Origin { get; set; }
    }
    [SchemaGen]
    public partial class Person : SchemaEntityRoot
    {
        [SchemaGen]
        public string Accreditation { get; set; }
        [SchemaGen]
        public string Address { get; set; }
        [SchemaGen]
        public string Address2 { get; set; }
        [SchemaGen]
        public string Address3 { get; set; }
        [SchemaGen]
        public string AddressFull { get; set; }
        private Attachment[] _Attachments;
        [SchemaGen]
        public Attachment[] Attachments { get { return _Attachments ?? new Attachment[0]; } set { _Attachments = value; } }
        [SchemaGen]
        public string BadgeID { get; set; }
        [SchemaGen]
        public string Bio { get; set; }
        private Category[] _Categories;
        [SchemaGen]
        public Category[] Categories { get { return _Categories ?? new Category[0]; } set { _Categories = value; } }
        [SchemaGen]
        public string City { get; set; }
        [SchemaGen]
        public string Company { get; set; }
        [SchemaGen]
        public string Company2 { get; set; }
        [SchemaGen]
        public string Country { get; set; }
        [SchemaGen]
        public string Email { get; set; }
        [SchemaGen]
        public string Email2 { get; set; }
        [SchemaGen]
        public string Facebook { get; set; }
        [SchemaGen]
        public string Fax { get; set; }
        [SchemaGen]
        public string FirstName { get; set; }
        [SchemaGen]
        public bool IsAttendee { get; set; }
        [SchemaGen]
        public bool IsContact { get; set; }
        [SchemaGen]
        public bool IsExhibitor { get; set; }
        [SchemaGen]
        public bool IsMember { get; set; }
        [SchemaGen]
        public bool IsSpeaker { get; set; }
        [SchemaGen]
        public bool IsSponsor { get; set; }
        [SchemaGen]
        public string LastName { get; set; }
        [SchemaGen]
        public decimal? Latitude { get; set; }
        [SchemaGen]
        public string LinkedIn { get; set; }
        [SchemaGen]
        public decimal? Longitude { get; set; }
        [SchemaGen]
        public string MemberNumber { get; set; }
        [SchemaGen]
        public string MemberType { get; set; }
        [SchemaGen]
        public string Middle { get; set; }
        [SchemaGen]
        public string Mobile { get; set; }
        [SchemaGen]
        public string NickName { get; set; }
        private PersonActivity[] _PersonActivities;
        [SchemaGen]
        public PersonActivity[] PersonActivities { get { return _PersonActivities ?? new PersonActivity[0]; } set { _PersonActivities = value; } }
        private PersonBeacon[] _PersonBeacons;
        [SchemaGen]
        public PersonBeacon[] PersonBeacons { get { return _PersonBeacons ?? new PersonBeacon[0]; } set { _PersonBeacons = value; } }
        private PersonCompany[] _PersonCompanies;
        [SchemaGen]
        public PersonCompany[] PersonCompanies { get { return _PersonCompanies ?? new PersonCompany[0]; } set { _PersonCompanies = value; } }
        private PersonLead[] _PersonLeads;
        [SchemaGen]
        public PersonLead[] PersonLeads { get { return _PersonLeads ?? new PersonLead[0]; } set { _PersonLeads = value; } }
        private PersonPurchase[] _PersonPurchases;
        [SchemaGen]
        public PersonPurchase[] PersonPurchases { get { return _PersonPurchases ?? new PersonPurchase[0]; } set { _PersonPurchases = value; } }
        private PersonRegistration[] _PersonRegistrations;
        [SchemaGen]
        public PersonRegistration[] PersonRegistrations { get { return _PersonRegistrations ?? new PersonRegistration[0]; } set { _PersonRegistrations = value; } }
        private PersonReservation[] _PersonReservations;
        [SchemaGen]
        public PersonReservation[] PersonReservations { get { return _PersonReservations ?? new PersonReservation[0]; } set { _PersonReservations = value; } }
        private PersonTransaction[] _PersonTransactions;
        [SchemaGen]
        public PersonTransaction[] PersonTransactions { get { return _PersonTransactions ?? new PersonTransaction[0]; } set { _PersonTransactions = value; } }
        [SchemaGen]
        public string Phone { get; set; }
        [SchemaGen]
        public string Phone2 { get; set; }
        [SchemaGen]
        public string PostalCode { get; set; }
        [SchemaGen]
        public string Prefix { get; set; }
        [SchemaGen]
        public string State { get; set; }
        [SchemaGen]
        public string Suffix { get; set; }
        [SchemaGen]
        public string Title { get; set; }
        [SchemaGen]
        public string Twitter { get; set; }
        [SchemaGen]
        public string Website { get; set; }
    }
    [SchemaGen]
    public partial class PersonActivity : SchemaEntity
    {
        [SchemaGen]
        public string ActivityType { get; set; }
        [SchemaGen]
        public string BoothID { get; set; }
        [SchemaGen]
        public string CompanyID { get; set; }
        [SchemaGen]
        public DateTime? FirstActivity { get; set; }
        [SchemaGen]
        public bool IsFavorite { get; set; }
        [SchemaGen]
        public bool IsLike { get; set; }
        [SchemaGen]
        public bool IsVisit { get; set; }
        [SchemaGen]
        public DateTime? LastActivity { get; set; }
        [SchemaGen]
        public string Name { get; set; }
        [SchemaGen]
        public string PersonID { get; set; }
        [SchemaGen]
        public string ProductID { get; set; }
        [SchemaGen]
        public TimeSpan? TotalDuration { get; set; }
    }
    [SchemaGen]
    public partial class PersonBeacon : SchemaEntity
    {
        [SchemaGen]
        public int Major { get; set; }
        [SchemaGen]
        public int Minor { get; set; }
        [SchemaGen]
        public DateTime? StartDate { get; set; }
        [SchemaGen]
        public string UUID { get; set; }
    }
    [SchemaGen]
    public partial class PersonCompany : SchemaEntity
    {
        [SchemaGen]
        public string CompanyID { get; set; }
    }
    [SchemaGen]
    public partial class PersonLead : SchemaEntity
    {
        [SchemaGen]
        public DateTime? CaptureDate { get; set; }
        [SchemaGen]
        public string CapturedID { get; set; }
        [SchemaGen]
        public string CapturedPersonID { get; set; }
    }
    [SchemaGen]
    public partial class PersonPurchase : SchemaEntity
    {
        [SchemaGen]
        public DateTime? CancelDate { get; set; }
        [SchemaGen]
        public string CancelType { get; set; }
        [SchemaGen]
        public string ProductID { get; set; }
        [SchemaGen]
        public DateTime? PurchaseDate { get; set; }
        [SchemaGen]
        public decimal? Quantity { get; set; }
        [SchemaGen]
        public string SourceCode { get; set; }
        [SchemaGen]
        public decimal? TotalFees { get; set; }
        [SchemaGen]
        public decimal? TotalPaid { get; set; }
        [SchemaGen]
        public DateTime? VerifyDate { get; set; }
        [SchemaGen]
        public string VerifyType { get; set; }
    }
    [SchemaGen]
    public partial class PersonRegistration : SchemaEntity
    {
        [SchemaGen]
        public DateTime? CancelDate { get; set; }
        [SchemaGen]
        public string CancelType { get; set; }
        [SchemaGen]
        public DateTime? RegistrationDate { get; set; }
        [SchemaGen]
        public string RegistrationTypeCode { get; set; }
        [SchemaGen]
        public string RegistrationTypeDescription { get; set; }
        [SchemaGen]
        public string SourceCode { get; set; }
        [SchemaGen]
        public decimal? TotalFees { get; set; }
        [SchemaGen]
        public decimal? TotalPaid { get; set; }
        [SchemaGen]
        public DateTime? VerifyDate { get; set; }
        [SchemaGen]
        public string VerifyType { get; set; }
    }
    [SchemaGen]
    public partial class PersonReservation : SchemaEntity
    {
        [SchemaGen]
        public string Block { get; set; }
        [SchemaGen]
        public DateTime? CancelDate { get; set; }
        [SchemaGen]
        public string CancelType { get; set; }
        [SchemaGen]
        public string Category { get; set; }
        [SchemaGen]
        public DateTime? CheckInDate { get; set; }
        [SchemaGen]
        public DateTime? CheckOutDate { get; set; }
        [SchemaGen]
        public string ConfirmationNumber { get; set; }
        [SchemaGen]
        public string FacilityID { get; set; }
        [SchemaGen]
        public decimal? NumberOfChildren { get; set; }
        [SchemaGen]
        public decimal? NumberOfGuests { get; set; }
        [SchemaGen]
        public decimal? Rate { get; set; }
        [SchemaGen]
        public DateTime? ReservationDate { get; set; }
        [SchemaGen]
        public string ReservationType { get; set; }
        [SchemaGen]
        public string SourceCode { get; set; }
        [SchemaGen]
        public string Status { get; set; }
        [SchemaGen]
        public string SubBlock { get; set; }
        [SchemaGen]
        public decimal? TotalFees { get; set; }
        [SchemaGen]
        public decimal? TotalPaid { get; set; }
    }
    [SchemaGen]
    public partial class PersonTransaction : SchemaEntity
    {
        [SchemaGen]
        public DateTime? CancelDate { get; set; }
        [SchemaGen]
        public string CancelType { get; set; }
        [SchemaGen]
        public string ProductID { get; set; }
        [SchemaGen]
        public DateTime? PurchaseDate { get; set; }
        [SchemaGen]
        public decimal? Quantity { get; set; }
        [SchemaGen]
        public string SourceCode { get; set; }
        [SchemaGen]
        public decimal? TotalFees { get; set; }
        [SchemaGen]
        public decimal? TotalPaid { get; set; }
        [SchemaGen]
        public string TransactionTypeCode { get; set; }
        [SchemaGen]
        public DateTime? VerifyDate { get; set; }
        [SchemaGen]
        public string VerifyType { get; set; }
    }
    [SchemaGen]
    public partial class Product : SchemaEntityRoot
    {
        [SchemaGen]
        public string Address { get; set; }
        [SchemaGen]
        public string Address2 { get; set; }
        [SchemaGen]
        public string Address3 { get; set; }
        [SchemaGen]
        public string AddressFull { get; set; }
        private Attachment[] _Attachments;
        [SchemaGen]
        public Attachment[] Attachments { get { return _Attachments ?? new Attachment[0]; } set { _Attachments = value; } }
        private Category[] _Categories;
        [SchemaGen]
        public Category[] Categories { get { return _Categories ?? new Category[0]; } set { _Categories = value; } }
        [SchemaGen]
        public string City { get; set; }
        [SchemaGen]
        public string Code { get; set; }
        [SchemaGen]
        public string Country { get; set; }
        [SchemaGen]
        public string Description { get; set; }
        [SchemaGen]
        public string Email { get; set; }
        [SchemaGen]
        public string Email2 { get; set; }
        [SchemaGen]
        public DateTime? EndDate { get; set; }
        [SchemaGen]
        public string Facebook { get; set; }
        [SchemaGen]
        public string Fax { get; set; }
        [SchemaGen]
        public bool IsEventProduct { get; set; }
        [SchemaGen]
        public decimal? Latitude { get; set; }
        [SchemaGen]
        public int? Limit { get; set; }
        [SchemaGen]
        public string LinkedIn { get; set; }
        [SchemaGen]
        public string Location { get; set; }
        [SchemaGen]
        public decimal? Longitude { get; set; }
        [SchemaGen]
        public string Mobile { get; set; }
        [SchemaGen]
        public string Name { get; set; }
        [SchemaGen]
        public string Phone { get; set; }
        [SchemaGen]
        public string Phone2 { get; set; }
        [SchemaGen]
        public string PostalCode { get; set; }
        private ProductPerson[] _ProductPersons;
        [SchemaGen]
        public ProductPerson[] ProductPersons { get { return _ProductPersons ?? new ProductPerson[0]; } set { _ProductPersons = value; } }
        [SchemaGen]
        public string ProductType { get; set; }
        [SchemaGen]
        public bool RegistrationRequired { get; set; }
        [SchemaGen]
        public int? Reserved { get; set; }
        [SchemaGen]
        public int? Sold { get; set; }
        [SchemaGen]
        public DateTime? SoldOutDate { get; set; }
        [SchemaGen]
        public DateTime? StartDate { get; set; }
        [SchemaGen]
        public string State { get; set; }
        private SubProduct[] _SubProducts;
        [SchemaGen]
        public SubProduct[] SubProducts { get { return _SubProducts ?? new SubProduct[0]; } set { _SubProducts = value; } }
        [SchemaGen]
        public string Twitter { get; set; }
        [SchemaGen]
        public string Website { get; set; }
    }
    [SchemaGen]
    public partial class ProductPerson : SchemaEntity
    {
        [SchemaGen]
        public string PersonID { get; set; }
    }
    [SchemaGen]
    public partial class SchemaBase
    {
    }
    [SchemaGen]
    public partial class SchemaEntity : SchemaBase
    {
        [SchemaGen]
        public string AnchorID { get; set; }
        private CompoSource[] _CompoSources;
        [SchemaGen]
        public CompoSource[] CompoSources { get { return _CompoSources ?? new CompoSource[0]; } set { _CompoSources = value; } }
        private string[] _OptOuts;
        [SchemaGen]
        public string[] OptOuts { get { return _OptOuts ?? new string[0]; } set { _OptOuts = value; } }
        [SchemaGen]
        public string SourceID { get; set; }
        [SchemaGen]
        public decimal SysPriority { get; set; }
    }
    [SchemaGen]
    public partial class SchemaEntityRoot : SchemaEntity
    {
        [SchemaGen]
        public bool Delete { get; set; }
        [SchemaGen]
        [BsonId]
        public string ID { get; set; }
    }
    [SchemaGen]
    public partial class SubProduct : SchemaEntity
    {
        [SchemaGen]
        public string ProductID { get; set; }
    }
}
