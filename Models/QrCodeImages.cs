using System.ComponentModel.DataAnnotations.Schema;

namespace JWTRefreshTokenInDotNet6.Models
{
    public class QrCodeImages
    {
        public int Id { get; set; }
        public byte[] Image {  get; set; }

        [ForeignKey("Lecture")]
        public int? lecId { get; set; }
        public Lecture Lecture { get; set; }


    }
}
