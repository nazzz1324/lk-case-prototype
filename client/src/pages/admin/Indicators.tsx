import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface Indicator {
  id: number;
  index: string;
  name: string;
}

export default function Indicators() {
  const [indicators, setIndicators] = useState<Indicator[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingIndicator, setEditingIndicator] = useState<Indicator | null>(null);
  const [formData, setFormData] = useState({
    index: "",
    name: "",
  });

  useEffect(() => {
    fetchIndicators();
  }, []);

  const fetchIndicators = async () => {
    try {
      setLoading(true);
      const response = await apiClient.getIndicators();
      if (response.data) {
        setIndicators(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch indicators:", error);
      toast.error("Не удалось загрузить индикаторы");
    } finally {
      setLoading(false);
    }
  };

  const filteredIndicators = indicators.filter((indicator) =>
    (indicator.name || "").toLowerCase().includes(searchTerm.toLowerCase()) ||
    (indicator.index || "").toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (indicator?: Indicator) => {
    if (indicator) {
      setEditingIndicator(indicator);
      setFormData({
        index: indicator.index,
        name: indicator.name,
      });
    } else {
      setEditingIndicator(null);
      setFormData({
        index: "",
        name: "",
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      if (editingIndicator) {
        await apiClient.updateIndicator({
          id: editingIndicator.id,
          index: formData.index,
          name: formData.name,
        });
        toast.success("Индикатор обновлен");
      } else {
        await apiClient.createIndicator({
          index: formData.index,
          name: formData.name,
        });
        toast.success("Индикатор создан");
      }
      setIsDialogOpen(false);
      fetchIndicators();
    } catch (error) {
      console.error("Failed to save indicator:", error);
      toast.error("Не удалось сохранить индикатор");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Вы уверены, что хотите удалить этот индикатор?")) return;
    try {
      await apiClient.deleteIndicator(id);
      toast.success("Индикатор удален");
      fetchIndicators();
    } catch (error) {
      toast.error("Не удалось удалить индикатор");
    }
  };

  if (loading) return <div className="p-8 text-center">Загрузка...</div>;

  return (
    <div className="space-y-6">
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
                <th className="px-6 py-4 text-left text-sm font-semibold">Код</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Название</th>
                <th className="px-6 py-4 text-left text-sm font-semibold">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredIndicators.map((indicator) => (
                <tr key={indicator.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm text-muted-foreground">{indicator.id}</td>
                  <td className="px-6 py-4 text-sm font-medium">{indicator.index}</td>
                  <td className="px-6 py-4 text-sm">{indicator.name}</td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(indicator)} className="p-2 text-primary hover:bg-secondary rounded-lg">
                        <Edit2 size={16} />
                      </button>
                      <button onClick={() => handleDelete(indicator.id)} className="p-2 text-destructive hover:bg-secondary rounded-lg">
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
            <DialogTitle>{editingIndicator ? `Редактировать (ID: ${editingIndicator.id})` : "Добавить индикатор"}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label>Код</Label>
              <Input value={formData.index} onChange={(e) => setFormData({ ...formData, index: e.target.value })} placeholder="УК-1.1" />
            </div>
            <div className="space-y-2">
              <Label>Название</Label>
              <Input value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} placeholder="Анализирует задачу..." />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave}>{editingIndicator ? "Сохранить" : "Добавить"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
