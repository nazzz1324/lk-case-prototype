import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface Group {
  id: number;
  name: string;
  studentCount?: number;
  curator?: string;
  students?: number[];
}

export default function Groups() {
  const [groups, setGroups] = useState<Group[]>([]);
  const [users, setUsers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<Group | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    curator: "",
    students: [] as number[],
  });

  useEffect(() => {
    fetchGroups();
    fetchUsers();
  }, []);

  const fetchGroups = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getGroups();
      if (response.data) {
        setGroups(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch groups:", error);
      toast.error("Не удалось загрузить группы");
    } finally {
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await apiClient.getUsers();
      if (response.data) {
        setUsers(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch users:", error);
    }
  };

  const filteredGroups = groups.filter((group) =>
    group.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (group?: Group) => {
    if (group) {
      setEditingGroup(group);
      setFormData({
        name: group.name,
        curator: group.curator || "",
        students: group.students || [],
      });
    } else {
      setEditingGroup(null);
      setFormData({
        name: "",
        curator: "",
        students: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    
    try {
      if (editingGroup) {
        await apiClient.updateGroup({
          id: editingGroup.id,
          name: formData.name,
          curator: formData.curator,
          students: formData.students,
        });
        toast.success("Группа обновлена");
      } else {
        await apiClient.createGroup({
          name: formData.name,
          curator: formData.curator,
          students: formData.students,
        });
        toast.success("Группа создана");
      }
      setIsDialogOpen(false);
      fetchGroups();
    } catch (error) {
      console.error("Failed to save group:", error);
      toast.error("Не удалось сохранить группу");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Вы уверены, что хотите удалить эту группу?")) {
      return;
    }
    
    try {
      await apiClient.deleteGroup(id);
      toast.success("Группа удалена");
      fetchGroups();
    } catch (error) {
      console.error("Failed to delete group:", error);
      toast.error("Не удалось удалить группу");
    }
  };

  const teachers = users.filter((u) => u.role === "Teacher");
  const students = users.filter((u) => u.role === "Student");

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">Загрузка...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header with Search */}
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="relative flex-1 w-full sm:w-auto">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
          <Input
            placeholder="Поиск по названию группы..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button
          onClick={() => handleOpenDialog()}
          className="bg-primary hover:bg-primary/90 text-primary-foreground whitespace-nowrap"
        >
          <Plus size={18} className="mr-2" />
          Добавить
        </Button>
      </div>

      {/* Table */}
      <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border bg-secondary">
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Название группы</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Студентов</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Куратор</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredGroups.map((group) => (
                <tr
                  key={group.id}
                  className="border-b border-border hover:bg-secondary transition-colors"
                >
                  <td className="px-6 py-4 text-sm font-medium text-foreground">{group.name}</td>
                  <td className="px-6 py-4 text-sm text-foreground">{group.studentCount || 0}</td>
                  <td className="px-6 py-4 text-sm text-muted-foreground">{group.curator || "—"}</td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleOpenDialog(group)}
                        className="p-2 hover:bg-secondary rounded-lg transition-colors text-primary"
                      >
                        <Edit2 size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(group.id)}
                        className="p-2 hover:bg-secondary rounded-lg transition-colors text-destructive"
                      >
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

      {/* Dialog */}
      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>
              {editingGroup ? "Редактировать группу" : "Добавить группу"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">Название группы</Label>
              <Input
                id="name"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                placeholder="ПИ-21-1"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="curator">Преподаватель-куратор</Label>
              <select
                id="curator"
                value={formData.curator}
                onChange={(e) => setFormData({ ...formData, curator: e.target.value })}
                className="w-full px-4 py-2 rounded-lg border border-border bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="">Выберите куратора</option>
                {teachers.map((teacher) => (
                  <option key={teacher.id} value={teacher.fullname}>  {/* ← ИЗМЕНЕНО */}
                    {teacher.fullname}  {/* ← ИЗМЕНЕНО */}
                  </option>
                ))}
              </select>
            </div>
            <div className="space-y-2">
              <Label>Студенты</Label>
              <div className="border border-border rounded-lg p-3 max-h-40 overflow-y-auto space-y-2">
                {students.map((student) => (
                  <label key={student.id} className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={formData.students.includes(student.id)}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setFormData({
                            ...formData,
                            students: [...formData.students, student.id],
                          });
                        } else {
                          setFormData({
                            ...formData,
                            students: formData.students.filter((id) => id !== student.id),
                          });
                        }
                      }}
                      className="w-4 h-4 rounded"
                    />
                    <span className="text-sm text-foreground">{student.fullname}</span>
                  </label>
                ))}
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>
              Отмена
            </Button>
            <Button onClick={handleSave} className="bg-primary hover:bg-primary/90 text-primary-foreground">
              {editingGroup ? "Сохранить" : "Добавить"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}