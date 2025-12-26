import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { api } from "@/lib/api";

interface Discipline {
  id: number;
  name: string;
  indicatorCount: number;
  indicators: number[];
}

export default function Disciplines() {
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);
  const [indicators, setIndicators] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingDiscipline, setEditingDiscipline] = useState<Discipline | null>(null);
  const [formData, setFormData] = useState({
    name: "",
    indicators: [] as number[],
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [discData, indData] = await Promise.all([
          api.admin.getDisciplines(),
          api.admin.getIndicators(),
        ]);
        setDisciplines(discData);
        setIndicators(indData);
      } catch (error) {
        console.error("Failed to fetch disciplines:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  const filteredDisciplines = disciplines.filter((discipline) =>
    discipline.name?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (discipline?: Discipline) => {
    if (discipline) {
      setEditingDiscipline(discipline);
      setFormData({
        name: discipline.name,
        indicators: discipline.indicators || [],
      });
    } else {
      setEditingDiscipline(null);
      setFormData({
        name: "",
        indicators: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      // api.admin.saveDiscipline(formData)
      setIsDialogOpen(false);
      const discData = await api.admin.getDisciplines();
      setDisciplines(discData);
    } catch (error) {
      console.error("Failed to save discipline:", error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Вы уверены?")) {
      setDisciplines(disciplines.filter((d) => d.id !== id));
    }
  };

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="relative flex-1 w-full sm:w-auto">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
          <Input
            placeholder="Поиск по названию дисциплины..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button onClick={() => handleOpenDialog()} className="bg-primary text-primary-foreground">
          <Plus size={18} className="mr-2" /> Добавить
        </Button>
      </div>

      <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border bg-secondary">
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Название дисциплины</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Индикаторов</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredDisciplines.map((discipline) => (
                <tr key={discipline.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm font-medium text-foreground">{discipline.name}</td>
                  <td className="px-6 py-4 text-sm text-foreground">{discipline.indicatorCount}</td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(discipline)} className="p-2 text-primary"><Edit2 size={16} /></button>
                      <button onClick={() => handleDelete(discipline.id)} className="p-2 text-destructive"><Trash2 size={16} /></button>
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
          <DialogHeader><DialogTitle>{editingDiscipline ? "Редактировать дисциплину" : "Добавить дисциплину"}</DialogTitle></DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">Название</Label>
              <Input id="name" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave} className="bg-primary text-primary-foreground">{editingDiscipline ? "Сохранить" : "Добавить"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
