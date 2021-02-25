//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialNetwork.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class objects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public objects()
        {
            this.articles = new HashSet<articles>();
            this.collections = new HashSet<collections>();
            this.commentaries = new HashSet<commentaries>();
            this.commentaries_to_objects_with_commentaries = new HashSet<commentaries_to_objects_with_commentaries>();
            this.files = new HashSet<files>();
            this.files_or_articles_to_collections = new HashSet<files_or_articles_to_collections>();
            this.objects_with_name_to_commentaries = new HashSet<objects_with_name_to_commentaries>();
            this.objects_with_name_to_messages = new HashSet<objects_with_name_to_messages>();
            this.objects_with_name_to_records = new HashSet<objects_with_name_to_records>();
            this.objects_with_name_to_showcases = new HashSet<objects_with_name_to_showcases>();
            this.ratings_to_objects_with_rating = new HashSet<ratings_to_objects_with_rating>();
        }
    
        public int id { get; set; }
        public int object_type_id { get; set; }
        public int user_id_from { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<articles> articles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<collections> collections { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<commentaries> commentaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<commentaries_to_objects_with_commentaries> commentaries_to_objects_with_commentaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<files> files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<files_or_articles_to_collections> files_or_articles_to_collections { get; set; }
        public virtual object_types object_types { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<objects_with_name_to_commentaries> objects_with_name_to_commentaries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<objects_with_name_to_messages> objects_with_name_to_messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<objects_with_name_to_records> objects_with_name_to_records { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<objects_with_name_to_showcases> objects_with_name_to_showcases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings_to_objects_with_rating> ratings_to_objects_with_rating { get; set; }
    }
}
