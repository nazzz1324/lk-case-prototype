import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface Competency {
  id: number;
  index: string;
  name: string;
  description?: string;
  indicators?: number[];
}

export default function Competencies() {
  const [competencies, setCompetencies] = useState<Competency[]>([]);
  const [indicators, setIndicators] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingCompetency, setEditingCompetency] = useState<Competency | null>(null);
  const [formData, setFormData] = useState({
    index: "",
    name: "",
    description: "",
    indicators: [] as number[],
  });

  useEffect(() => {
    fetchCompetencies();
    fetchIndicators();
  }, []);

  const fetchCompetencies = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getCompetences();
      if (response.data) {
        setCompetencies(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch competencies:", error);
      toast.error("Не удалось загрузить компетенции");
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

  const filteredCompetencies = competencies.filter((competency) =>
    competency.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    competency.index.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (competency?: Competency) => { 
    console.log('Открываем компетенцию:', competency);
    console.log('Её index:', competency?.index);
    console.log('Её поля:', Object.keys(competency || {}));
    if (competency) {
      setEditingCompetency(competency);
      setFormData({
        index: competency.index,
        name: competency.name,
        description: competency.description || "",
        indicators: competency.indicators || [],
      });
    } else {
      setEditingCompetency(null);
      setFormData({
        index: "",
        name: "",
        description: "",
        indicators: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    console.log('=== СОХРАНЕНИЕ КОМПЕТЕНЦИИ ===');
    console.log('formData:', formData);
    console.log('indicators:', formData.indicators);
    try {
      if (editingCompetency) {
        await apiClient.updateCompetence({
          id: editingCompetency.id,
          ...formData,
        });
        toast.success("Компетенция обновлена");
      } else {
        await apiClient.createCompetence(formData);
        toast.success("Компетенция создана");
      }
      setIsDialogOpen(false);
      fetchCompetencies();
    } catch (error) {
      console.error("Failed to save competency:", error);
      toast.error("Не удалось сохранить компетенцию");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Вы уверены, что хотите удалить эту компетенцию?")) {
      return;
    }
    
    try {
      await apiClient.deleteCompetence(id);
      toast.success("Компетенция удалена");
      fetchCompetencies();
    } catch (error) {
      console.error("Failed to delete competency:", error);
      toast.error("Не удалось удалить компетенцию");
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
            placeholder="Поиск по коду или названию..."
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
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Код</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Название</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Описание</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredCompetencies.map((competency) => (
                <tr
                  key={competency.id}
                  className="border-b border-border hover:bg-secondary transition-colors"
                >
                  <td className="px-6 py-4 text-sm font-medium text-foreground">{competency.index}</td>
                  <td className="px-6 py-4 text-sm text-foreground">{competency.name}</td>
                  <td className="px-6 py-4 text-sm text-muted-foreground max-w-md truncate">
                    {competency.description || "—"}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleOpenDialog(competency)}
                        className="p-2 hover:bg-secondary rounded-lg transition-colors text-primary"
                      >
                        <Edit2 size={16} /> 
                      </button>
                      <button
                        onClick={() => handleDelete(competency.id)}
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
              {editingCompetency ? "Редактировать компетенцию" : "Добавить компетенцию"}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="code">Код</Label>
              <Input
                id="code"
                value={formData.index}
                onChange={(e) => setFormData({ ...formData, index: e.target.value })}
                placeholder="УК-1"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="name">Название</Label>
              <Input
                id="name"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                placeholder="Системное и критическое мышление"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="description">Описание</Label>
              <Textarea
                id="description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                placeholder="Описание компетенции..."
                rows={3}
              />
            </div>
            <div className="space-y-2">
              <Label>Индикаторы</Label>
              <div className="border border-border rounded-lg p-3 max-h-40 overflow-y-auto space-y-2">
                {indicators.map((indicator) => (
                  <label key={indicator.id} className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="checkbox"
                      checked={formData.indicators.includes(indicator.id)}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setFormData({
                            ...formData,
                            indicators: [...formData.indicators, indicator.id],
                          });
                        } else {
                          setFormData({
                            ...formData,
                            indicators: formData.indicators.filter((id) => id !== indicator.id),
                          });
                        }
                      }}
                      className="w-4 h-4 rounded"
                    />
                    <span className="text-sm text-foreground">{indicator.index} - {indicator.name}</span>
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
              {editingCompetency ? "Сохранить" : "Добавить"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
