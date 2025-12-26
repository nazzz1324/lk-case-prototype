import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search, CheckCircle2, XCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface User {
  id?: number;
  userId?: number;
  userID?: number;
  fullname: string;
  firstname?: string;
  lastname?: string;
  middlename?: string;
  email: string;
  role: string;
  roleId: number;
  isActive: boolean;
}


const ROLE_NAME_MAP: Record<number, string> = {
  1: "Student",
  2: "Teacher",
  3: "Admin"
};

const ROLE_LABEL_MAP: Record<number, string> = {
  1: "Студент",
  2: "Преподаватель",
  3: "Администратор"
};

export default function Users() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [roleFilter, setRoleFilter] = useState("all");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [formData, setFormData] = useState({
    firstname: "",
    lastname: "",
    middlename: "",
    email: "",
    password: "",
    roleId: 1,
    isActive: true,
  });

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getUsers();
      if (response.data) {
        setUsers(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch users:", error);
    } finally {
      setLoading(false);
    }
  };

  const getUserId = (user: User): number => {
    return user.id || user.userId || user.userID || 0;
  };

  const filteredUsers = users.filter((user) => {
    const matchesSearch =
      (user.fullname || "").toLowerCase().includes(searchTerm.toLowerCase()) ||
      (user.email || "").toLowerCase().includes(searchTerm.toLowerCase());
    const userRoleName = ROLE_NAME_MAP[user.roleId] || user.role;
    const matchesRole = roleFilter === "all" || userRoleName === roleFilter;
    return matchesSearch && matchesRole;
  });

  const handleOpenDialog = (user?: User) => {
    if (user) {
      setEditingUser(user);
      const nameParts = user.fullname ? user.fullname.split(/\s+/) : ["", "", ""];
      setFormData({
        firstname: user.firstname || nameParts[1] || "",
        lastname: user.lastname || nameParts[0] || "",
        middlename: user.middlename || nameParts[2] || "",
        email: user.email || "",
        password: "",
        roleId: user.roleId || 1,
        isActive: user.isActive ?? true,
      });
    } else {
      setEditingUser(null);
      setFormData({
        firstname: "",
        lastname: "",
        middlename: "",
        email: "",
        password: "",
        roleId: 1,
        isActive: true,
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      const payload: any = {
        firstname: formData.firstname,
        lastname: formData.lastname,
        middlename: formData.middlename,
        email: formData.email,
        roleId: formData.roleId,
        isActive: formData.isActive
      };

      if (editingUser) {
        // При редактировании ID передается только для идентификации, но не меняется пользователем
        payload.id = getUserId(editingUser);
        if (formData.password) payload.password = formData.password;
        await apiClient.updateUser(payload);
        toast.success("Пользователь обновлен");
      } else {
        // При создании ID НЕ ОТПРАВЛЯЕТСЯ, его присвоит БД
        if (!formData.password) {
          toast.error("Пароль обязателен");
          return;
        }
        payload.password = formData.password;
        await apiClient.register(payload);
        toast.success("Пользователь зарегистрирован");
      }
      setIsDialogOpen(false);
      fetchUsers();
    } catch (error: any) {
      console.error("Save error:", error);
      toast.error("Ошибка при сохранении");
    }
  };

  const handleDelete = async (user: User) => {
    const id = getUserId(user);
    if (!id || !confirm("Удалить пользователя?")) return;
    try {
      await apiClient.deleteUser(id);
      toast.success("Пользователь удален");
      fetchUsers();
    } catch (error) {
      toast.error("Не удалось удалить пользователя");
    }
  };

  if (loading) return <div className="p-8 text-center">Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="flex-1 flex gap-4 w-full sm:w-auto">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
            <Input
              placeholder="Поиск по ФИО или Email..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          <select
            value={roleFilter}
            onChange={(e) => setRoleFilter(e.target.value)}
            className="px-4 py-2 rounded-lg border border-border bg-background"
          >
            <option value="all">Все роли</option>
            <option value="Admin">Администратор</option>
            <option value="Teacher">Преподаватель</option>
            <option value="Student">Студент</option>
          </select>
        </div>
        <Button onClick={() => handleOpenDialog()}>
          <Plus size={18} className="mr-2" /> Добавить
        </Button>
      </div>

      <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border bg-secondary">
                <th className="px-6 py-4 text-left text-sm font-semibold">ID</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">ФИО</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Email</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Роль</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Статус</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map((user) => (
                <tr key={getUserId(user) || user.email} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm text-muted-foreground">{getUserId(user)}</td>
                  <td className="px-6 py-4 text-sm font-medium">
                    {user.fullname || `${user.lastname || ""} ${user.firstname || ""} ${user.middlename || ""}`.trim() || "—"}
                  </td>
                  <td className="px-6 py-4 text-sm text-muted-foreground">{user.email}</td>
                  <td className="px-6 py-4 text-sm">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                      user.roleId === 3 ? "bg-red-100 text-red-700" :
                      user.roleId === 2 ? "bg-blue-100 text-blue-700" :
                      "bg-green-100 text-green-700"
                    }`}>
                      {ROLE_LABEL_MAP[user.roleId] || user.role}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <span className={`flex items-center gap-1.5 ${user.isActive ? "text-green-600" : "text-red-500"}`}>
                      {user.isActive ? <CheckCircle2 size={14} /> : <XCircle size={14} />}
                      {user.isActive ? "Активен" : "Неактивен"}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(user)} className="p-2 text-primary hover:bg-secondary rounded-lg">
                        <Edit2 size={16} />
                      </button>
                      <button onClick={() => handleDelete(user)} className="p-2 text-destructive hover:bg-secondary rounded-lg">
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{editingUser ? `Редактирование пользователя (ID: ${getUserId(editingUser)})` : "Добавление нового пользователя"}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="grid grid-cols-1 gap-4">
              <div className="space-y-2">
                <Label>Фамилия</Label>
                <Input value={formData.lastname} onChange={(e) => setFormData({ ...formData, lastname: e.target.value })} placeholder="Иванов" />
              </div>
              <div className="space-y-2">
                <Label>Имя</Label>
                <Input value={formData.firstname} onChange={(e) => setFormData({ ...formData, firstname: e.target.value })} placeholder="Иван" />
              </div>
              <div className="space-y-2">
                <Label>Отчество</Label>
                <Input value={formData.middlename} onChange={(e) => setFormData({ ...formData, middlename: e.target.value })} placeholder="Иванович" />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Email</Label>
              <Input type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} placeholder="ivanov@example.com" />
            </div>
            <div className="space-y-2">
              <Label>{editingUser ? "Новый пароль (оставьте пустым, если не меняете)" : "Пароль"}</Label>
              <Input type="password" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} placeholder="••••••••" />
            </div>
            <div className="flex items-center justify-between gap-4">
              <div className="space-y-2 flex-1">
                <Label>Роль</Label>
                <select
                  value={formData.roleId}
                  onChange={(e) => setFormData({ ...formData, roleId: parseInt(e.target.value) })}
                  className="w-full px-4 py-2 rounded-lg border border-border bg-background"
                >
                  <option value={3}>Администратор</option>
                  <option value={2}>Преподаватель</option>
                  <option value={1}>Студент</option>
                </select>
              </div>
              <div className="space-y-2">
                <Label>Статус</Label>
                <div className="flex items-center h-10">
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input 
                      type="checkbox" 
                      className="sr-only peer" 
                      checked={formData.isActive}
                      onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                    />
                    <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-primary/20 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary"></div>
                    <span className="ml-3 text-sm font-medium text-foreground">{formData.isActive ? "Активен" : "Неактивен"}</span>
                  </label>
                </div>
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave}>{editingUser ? "Сохранить изменения" : "Зарегистрировать"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
