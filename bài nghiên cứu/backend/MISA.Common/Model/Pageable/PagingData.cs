namespace MISA.Common.Model.Pageable;

/**
 * Cau hinh du lieu cho danh sach object kem pageable
 */
public class PagingData<T>
{
    public IEnumerable<T>? Data { get; set; }
    public Pageable? Pageable { get; set; }
}