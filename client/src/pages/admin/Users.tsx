import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { api } from "@/lib/api";

interface User {
  id: number;
  name: string;
  email: string;
  role: string;
  status: string;
  group?: string | null;
  discipline?: string[] | null;
}

export default function Users() {
  const [users, setUsers] = useState<User[]>([]);
  const [disciplines, setDisciplines] = useState<any[]>([]);
  const [groups, setGroups] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [roleFilter, setRoleFilter] = useState("all");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    role: "student",
    group: "",
    discipline: [] as string[],
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [usersData, disciplinesData, groupsData] = await Promise.all([
          api.admin.getUsers(),
          api.admin.getDisciplines(),
          api.admin.getGroups(),
        ]);
        setUsers(usersData);
        setDisciplines(disciplinesData);
        setGroups(groupsData);
      } catch (error) {
        console.error("Failed to fetch users:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  const filteredUsers = users.filter((user) => {
    const matchesSearch =
      user.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesRole = roleFilter === "all" || user.role === roleFilter;
    return matchesSearch && matchesRole;
  });

  const handleOpenDialog = (user?: User) => {
    if (user) {
      setEditingUser(user);
      setFormData({
        name: user.name,
        email: user.email,
        role: user.role,
        group: user.group || "",
        discipline: user.discipline || [],
      });
    } else {
      setEditingUser(null);
      setFormData({
        name: "",
        email: "",
        role: "student",
        group: "",
        discipline: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      // Здесь должна быть логика сохранения через API
      // api.admin.saveUser(formData)
      setIsDialogOpen(false);
      // Перезагрузка данных
      const usersData = await api.admin.getUsers();
      setUsers(usersData);
    } catch (error) {
      console.error("Failed to save user:", error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Вы уверены, что хотите удалить этого пользователя?")) {
      try {
        // api.admin.deleteUser(id)
        setUsers(users.filter((u) => u.id !== id));
      } catch (error) {
        console.error("Failed to delete user:", error);
      }
    }
  };

  const getRoleLabel = (role: string) => {
    const roleMap: Record<string, string> = {
      admin: "Администратор",
      teacher: "Преподаватель",
      student: "Студент",
    };
    return roleMap[role] || role;
  };

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="flex-1 flex gap-4 w-full sm:w-auto">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
            <Input
              placeholder="Поиск по имени или email..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          <select
            value={roleFilter}
            onChange={(e) => setRoleFilter(e.target.value)}
            className="px-4 py-2 rounded-lg border border-border bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
          >
            <option value="all">Все роли</option>
            <option value="admin">Администратор</option>
            <option value="teacher">Преподаватель</option>
            <option value="student">Студент</option>
          </select>
        </div>
        <Button onClick={() => handleOpenDialog()} className="bg-primary hover:bg-primary/90 text-primary-foreground">
          <Plus size={18} className="mr-2" />
          Добавить
        </Button>
      </div>

      <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border bg-secondary">
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">ФИО</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Email</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Роль</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Статус</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map((user) => (
                <tr key={user.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm text-foreground">{user.name}</td>
                  <td className="px-6 py-4 text-sm text-muted-foreground">{user.email}</td>
                  <td className="px-6 py-4 text-sm text-foreground">{getRoleLabel(user.role)}</td>
                  <td className="px-6 py-4 text-sm">
                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${user.status === "active" ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
                      {user.status === "active" ? "Активен" : "Заблокирован"}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(user)} className="p-2 hover:bg-secondary rounded-lg text-primary">
                        <Edit2 size={16} />
                      </button>
                      <button onClick={() => handleDelete(user.id)} className="p-2 hover:bg-secondary rounded-lg text-destructive">
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
            <DialogTitle>{editingUser ? "Редактировать пользователя" : "Добавить пользователя"}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">ФИО</Label>
              <Input id="name" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="role">Роль</Label>
              <select
                id="role"
                value={formData.role}
                onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                className="w-full px-4 py-2 rounded-lg border border-border bg-background"
              >
                <option value="student">Студент</option>
                <option value="teacher">Преподаватель</option>
                <option value="admin">Администратор</option>
              </select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave} className="bg-primary text-primary-foreground">
              {editingUser ? "Сохранить" : "Добавить"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
