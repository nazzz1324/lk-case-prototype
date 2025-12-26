import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface Discipline {
  id: number;
  index: string;
  name: string;
  indicatorCount: number;
  indicatorIds?: number[];
}

export default function Disciplines() {
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);
  const [indicators, setIndicators] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingDiscipline, setEditingDiscipline] = useState<Discipline | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    index: "",
    indicatorIds: [] as number[],
  });

  useEffect(() => {
    fetchDisciplines();
    fetchIndicators();
  }, []);

  const fetchDisciplines = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getDisciplines();
      if (response.data) {
        setDisciplines(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch disciplines:", error);
      toast.error("Не удалось загрузить дисциплины");
    } finally {
      setLoading(false);
    }
  };

  const fetchIndicators = async () => {
    try {
      const response = await apiClient.getIndicators();
      if (response.data) {
        setIndicators(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch indicators:", error);
    }
  };

  const filteredDisciplines = disciplines.filter((discipline) => {
    const matchesSearch = 
      discipline.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      discipline.index.toLowerCase().includes(searchTerm.toLowerCase());
    return matchesSearch;
  });

  const handleOpenDialog = (discipline?: Discipline) => {
    if (discipline) {
      setEditingDiscipline(discipline);
      setFormData({
        name: discipline.name,
        index: discipline.index,
        indicatorIds: discipline.indicatorIds || [],
      });
    } else {
      setEditingDiscipline(null);
      setFormData({
        name: "",
        index: "",
        indicatorIds: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      if (editingDiscipline) {
        await apiClient.updateDiscipline({
          id: editingDiscipline.id,
          name: formData.name,
          index: formData.index,
          indicatorIds: formData.indicatorIds,
        });
        toast.success("Дисциплина обновлена");
      } else {
        await apiClient.createDiscipline({
          name: formData.name,
          index: formData.index,
          indicatorIds: formData.indicatorIds,
        });
        toast.success("Дисциплина создана");
      }
      setIsDialogOpen(false);
      fetchDisciplines();
    } catch (error) {
      console.error("Failed to save discipline:", error);
      toast.error("Не удалось сохранить дисциплину");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Вы уверены, что хотите удалить эту дисциплину?")) return;
    try {
      await apiClient.deleteDiscipline(id);
      toast.success("Дисциплина удалена");
      fetchDisciplines();
    } catch (error) {
      toast.error("Не удалось удалить дисциплину");
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
              placeholder="Поиск по названию или индексу..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
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
                <th className="px-6 py-4 text-left text-sm font-semibold">Индекс</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Название дисциплины</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Индикаторов</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredDisciplines.map((discipline) => (
                <tr key={discipline.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm text-muted-foreground">{discipline.id}</td>
                  <td className="px-6 py-4 text-sm font-medium">{discipline.index}</td>
                  <td className="px-6 py-4 text-sm">{discipline.name}</td>
                  <td className="px-6 py-4 text-sm">{discipline.indicatorCount}</td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(discipline)} className="p-2 text-primary hover:bg-secondary rounded-lg">
                        <Edit2 size={16} />
                      </button>
                      <button onClick={() => handleDelete(discipline.id)} className="p-2 text-destructive hover:bg-secondary rounded-lg">
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
            <DialogTitle>{editingDiscipline ? `Редактировать (ID: ${editingDiscipline.id})` : "Добавить дисциплину"}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label>Индекс</Label>
              <Input value={formData.index} onChange={(e) => setFormData({ ...formData, index: e.target.value })} placeholder="Б.О.1.1" />
            </div>
            <div className="space-y-2">
              <Label>Название</Label>
              <Input value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} placeholder="Основы программирования" />
            </div>
            <div className="space-y-2">
              <Label>Выбор Индикаторов</Label>
              <div className="border border-border rounded-lg p-3 max-h-48 overflow-y-auto space-y-2">
                {indicators.map((indicator) => (
                  <label key={indicator.id} className="flex items-center gap-2 cursor-pointer hover:bg-secondary p-1 rounded">
                    <input
                      type="checkbox"
                      checked={formData.indicatorIds.includes(indicator.id)}
                      onChange={(e) => {
                        const id = indicator.id;
                        setFormData({
                          ...formData,
                          indicatorIds: e.target.checked 
                            ? [...formData.indicatorIds, id] 
                            : formData.indicatorIds.filter(i => i !== id)
                        });
                      }}
                      className="w-4 h-4 rounded border-gray-300 text-primary focus:ring-primary"
                    />
                    <span className="text-sm">{indicator.code} - {indicator.name}</span>
                  </label>
                ))}
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave}>{editingDiscipline ? "Сохранить" : "Добавить"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
