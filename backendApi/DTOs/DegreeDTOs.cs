using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateDegreeDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of degree name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        // Bangla version of degree level
        [MaxLength(50)]
        public string? LevelBangla { get; set; }

        // Institution type (e.g., University, College, School, Board)
        [MaxLength(100)]
        public string? InstitutionType { get; set; }

        // Bangla version of institution type
        [MaxLength(100)]
        public string? InstitutionTypeBangla { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class UpdateDegreeDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of degree name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        // Bangla version of degree level
        [MaxLength(50)]
        public string? LevelBangla { get; set; }

        // Institution type (e.g., University, College, School, Board)
        [MaxLength(100)]
        public string? InstitutionType { get; set; }

        // Bangla version of institution type
        [MaxLength(100)]
        public string? InstitutionTypeBangla { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class DegreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of degree name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string Level { get; set; } = string.Empty;
        public string? LevelBangla { get; set; }
        public string? InstitutionType { get; set; }
        public string? InstitutionTypeBangla { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DegreeListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public string Level { get; set; } = string.Empty;
        public string? LevelBangla { get; set; }
        public string? InstitutionType { get; set; }
        public string? InstitutionTypeBangla { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // Common Bangladesh educational degrees and certificates
    public static class BangladeshDegrees
    {
        public static readonly List<DegreeTemplate> CommonDegrees = new()
        {
            // Secondary School Certificate (SSC)
            new DegreeTemplate("Secondary School Certificate", "মাধ্যমিক স্কুল সার্টিফিকেট", "SSC", "এসএসসি", "Board", "বোর্ড"),
            new DegreeTemplate("Higher Secondary Certificate", "উচ্চ মাধ্যমিক সার্টিফিকেট", "HSC", "এইচএসসি", "Board", "বোর্ড"),
            
            // Bachelor Degrees
            new DegreeTemplate("Bachelor of Arts", "কলা বিভাগে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Science", "বিজ্ঞান বিভাগে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Commerce", "বাণিজ্য বিভাগে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Business Administration", "ব্যবসায় প্রশাসনে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Engineering", "প্রকৌশলে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Technology", "প্রযুক্তিতে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Medicine", "চিকিৎসায় স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Laws", "আইনে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Education", "শিক্ষায় স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Bachelor of Social Work", "সমাজকর্মে স্নাতক", "Bachelor", "স্নাতক", "University", "বিশ্ববিদ্যালয়"),
            
            // Master Degrees
            new DegreeTemplate("Master of Arts", "কলা বিভাগে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Science", "বিজ্ঞান বিভাগে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Commerce", "বাণিজ্য বিভাগে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Business Administration", "ব্যবসায় প্রশাসনে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Engineering", "প্রকৌশলে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Technology", "প্রযুক্তিতে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Laws", "আইনে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Education", "শিক্ষায় স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Master of Social Work", "সমাজকর্মে স্নাতকোত্তর", "Master", "স্নাতকোত্তর", "University", "বিশ্ববিদ্যালয়"),
            
            // Doctoral Degrees
            new DegreeTemplate("Doctor of Philosophy", "দর্শনে ডক্টরেট", "PhD", "পিএইচডি", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Doctor of Medicine", "চিকিৎসায় ডক্টরেট", "MD", "এমডি", "University", "বিশ্ববিদ্যালয়"),
            new DegreeTemplate("Doctor of Engineering", "প্রকৌশলে ডক্টরেট", "PhD", "পিএইচডি", "University", "বিশ্ববিদ্যালয়"),
            
            // Professional Certificates
            new DegreeTemplate("Diploma in Engineering", "প্রকৌশলে ডিপ্লোমা", "Diploma", "ডিপ্লোমা", "Polytechnic", "পলিটেকনিক"),
            new DegreeTemplate("Diploma in Commerce", "বাণিজ্যে ডিপ্লোমা", "Diploma", "ডিপ্লোমা", "College", "কলেজ"),
            new DegreeTemplate("Certificate Course", "সার্টিফিকেট কোর্স", "Certificate", "সার্টিফিকেট", "Institute", "ইনস্টিটিউট"),
            new DegreeTemplate("Professional Certificate", "পেশাগত সার্টিফিকেট", "Certificate", "সার্টিফিকেট", "Institute", "ইনস্টিটিউট"),
            
            // Technical Certificates
            new DegreeTemplate("Technical Certificate", "প্রযুক্তিগত সার্টিফিকেট", "Certificate", "সার্টিফিকেট", "Technical Institute", "প্রযুক্তি ইনস্টিটিউট"),
            new DegreeTemplate("Vocational Certificate", "বৃত্তিমূলক সার্টিফিকেট", "Certificate", "সার্টিফিকেট", "Vocational Institute", "বৃত্তিমূলক ইনস্টিটিউট"),
            
            // Language Certificates
            new DegreeTemplate("English Language Certificate", "ইংরেজি ভাষা সার্টিফিকেট", "Certificate", "সার্টিফিকেট", "Language Institute", "ভাষা ইনস্টিটিউট"),
            new DegreeTemplate("Arabic Language Certificate", "আরবি ভাষা সার্টিফিকেট", "Certificate", "সার্টিফিকেট", "Language Institute", "ভাষা ইনস্টিটিউট"),
            
            // Religious Education
            new DegreeTemplate("Dakhil", "দাখিল", "Dakhil", "দাখিল", "Madrasah", "মাদ্রাসা"),
            new DegreeTemplate("Alim", "আলিম", "Alim", "আলিম", "Madrasah", "মাদ্রাসা"),
            new DegreeTemplate("Fazil", "ফাজিল", "Fazil", "ফাজিল", "Madrasah", "মাদ্রাসা"),
            new DegreeTemplate("Kamil", "কামিল", "Kamil", "কামিল", "Madrasah", "মাদ্রাসা")
        };
    }

    public class DegreeTemplate
    {
        public string Name { get; set; }
        public string NameBangla { get; set; }
        public string Level { get; set; }
        public string LevelBangla { get; set; }
        public string InstitutionType { get; set; }
        public string InstitutionTypeBangla { get; set; }

        public DegreeTemplate(string name, string nameBangla, string level, string levelBangla, string institutionType, string institutionTypeBangla)
        {
            Name = name;
            NameBangla = nameBangla;
            Level = level;
            LevelBangla = levelBangla;
            InstitutionType = institutionType;
            InstitutionTypeBangla = institutionTypeBangla;
        }
    }
}
