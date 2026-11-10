namespace ConnectDbAvalonia.Models
{
    public partial class User
    {
        public override string ToString()
        {
            // Если роль подгрузилась из базы, пишем её. Если нет — пишем "Без роли"
            string role = Role != null ? Role.RoleName : "Без роли";
            return $"{Login} ({role})";
        }
    }
}