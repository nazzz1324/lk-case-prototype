import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface ProfessionalRole {
  id: number;
  name: string;
  description?: string;
  competencies?: number[];
}

export default function Professions() {
  const [professions, setProfessions] = useState<ProfessionalRole[]>([]);
  const [competencies, setCompetencies] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingProfession, setEditingProfession] = useState<ProfessionalRole | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    competencies: [] as number[],
  });

  useEffect(() => {
    fetchProfessions();
    fetchCompetencies();
  }, []);

  const fetchProfessions = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getProfessionalRoles();
      if (response.data) {
        setProfessions(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch professions:", error);
      toast.error("Не удалось загрузить профессии");
    } finally {
      setLoading(false);
    }
  };

  const fetchCompetencies = async () => {
    try {
      const response = await apiClient.getCompetences();
      if (response.data) {
        setCompetencies(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch competencies:", error);
    }
  };

  const filteredProfessions = professions.filter((profession) =>
    profession.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (profession?: ProfessionalRole) => {
    console.log('Профроль при открытии:', profession);
    console.log('Её Competencies:', profession?.competencies);
    if (profession) {
      setEditingProfession(profession);
      setFormData({
        name: profession.name,
        description: profession.description || "",
        competencies: profession.competencies || [],
      });
    } else {
      setEditingProfession(null);
      setFormData({
        name: "",
        description: "",
        competencies: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      if (editingProfession) {
        await apiClient.updateProfessionalRole({
          id: editingProfession.id,
          ...formData,
        });
        toast.success("Профессия обновлена");
      } else {
        await apiClient.createProfessionalRole(formData);
        toast.success("Профессия создана");
      }
      setIsDialogOpen(false);
      fetchProfessions();
    } catch (error) {
      console.error("Failed to save profession:", error);
      toast.error("Не удалось сохранить профессию");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Вы уверены, что хотите удалить эту профессию?")) {
      return;
    }
    
    try {
      await apiClient.deleteProfessionalRole(id);
      toast.success("Профессия удалена");
      fetchProfessions();
    } catch (error) {
      console.error("Failed to delete profession:", error);
      toast.error("Не удалось удалить профессию");
    }
  };

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
            placeholder="Поиск по названию профессии..."
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
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Название</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Описание</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredProfessions.map((profession) => (
                <tr
                  key={profession.id}
                  className="border-b border-border hover:bg-secondary transition-colors"
                >
                  <td className="px-6 py-4 text-sm font-medium text-foreground">{profession.name}</td>
                  <td className="px-6 py-4 text-sm text-muted-foreground max-w-md truncate">
                    {profession.description || "—"}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleOpenDialog(profession)}
                        className="p-2 hover:bg-secondary rounded-lg transition-colors text-primary"
                      >
                        <Edit2 size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(profession.id)}
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
              {editingProfession ? "Редактировать профессию" : "Добавить профессию"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">Название</Label>
              <Input
                id="name"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                placeholder="Веб-разработчик"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="description">Описание</Label>
              <Textarea
                id="description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                placeholder="Описание профессии..."
                rows={3}
              />
            </div>
            <div className="space-y-2">
              <Label>Компетенции</Label>
              <div className="border border-border rounded-lg p-3 max-h-40 overflow-y-auto space-y-2">
                {competencies.map((competency) => (
                  <label key={competency.id} className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={formData.competencies.includes(competency.id)}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setFormData({
                            ...formData,
                            competencies: [...formData.competencies, competency.id],
                          });
                        } else {
                          setFormData({
                            ...formData,
                            competencies: formData.competencies.filter((id) => id !== competency.id),
                          });
                        }
                      }}
                      className="w-4 h-4 rounded"
                    />
                    <span className="text-sm text-foreground">{competency.index} - {competency.name}</span>
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
              {editingProfession ? "Сохранить" : "Добавить"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
