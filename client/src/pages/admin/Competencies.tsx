import { useState, useEffect } from "react";
import { Plus, Edit2, Trash2, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { api } from "@/lib/api";

interface Competency {
  id: number;
  code: string;
  name: string;
  description: string;
  indicators: number[];
}

export default function Competencies() {
  const [competencies, setCompetencies] = useState<Competency[]>([]);
  const [indicators, setIndicators] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingCompetency, setEditingCompetency] = useState<Competency | null>(null);
  const [formData, setFormData] = useState({
    code: "",
    name: "",
    description: "",
    indicators: [] as number[],
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [compData, indData] = await Promise.all([
          api.admin.getCompetencies(),
          api.admin.getIndicators(),
        ]);
        setCompetencies(compData);
        setIndicators(indData);
      } catch (error) {
        console.error("Failed to fetch competencies:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  const filteredCompetencies = competencies.filter((competency) =>
    competency.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    competency.code?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleOpenDialog = (competency?: Competency) => {
    if (competency) {
      setEditingCompetency(competency);
      setFormData({
        code: competency.code,
        name: competency.name,
        description: competency.description,
        indicators: competency.indicators || [],
      });
    } else {
      setEditingCompetency(null);
      setFormData({
        code: "",
        name: "",
        description: "",
        indicators: [],
      });
    }
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      // api.admin.saveCompetency(formData)
      setIsDialogOpen(false);
      const compData = await api.admin.getCompetencies();
      setCompetencies(compData);
    } catch (error) {
      console.error("Failed to save competency:", error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Вы уверены?")) {
      setCompetencies(competencies.filter((c) => c.id !== id));
    }
  };

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div className="relative flex-1 w-full sm:w-auto">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
          <Input
            placeholder="Поиск по названию или коду..."
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
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Код</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Название</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Описание</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-foreground">Действия</th>
              </tr>
            </thead>
            <tbody>
              {filteredCompetencies.map((competency) => (
                <tr key={competency.id} className="border-b border-border hover:bg-secondary transition-colors">
                  <td className="px-6 py-4 text-sm font-medium text-primary">{competency.code}</td>
                  <td className="px-6 py-4 text-sm font-medium text-foreground">{competency.name}</td>
                  <td className="px-6 py-4 text-sm text-muted-foreground truncate">{competency.description}</td>
                  <td className="px-6 py-4 text-sm">
                    <div className="flex gap-2">
                      <button onClick={() => handleOpenDialog(competency)} className="p-2 text-primary"><Edit2 size={16} /></button>
                      <button onClick={() => handleDelete(competency.id)} className="p-2 text-destructive"><Trash2 size={16} /></button>
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
          <DialogHeader><DialogTitle>{editingCompetency ? "Редактировать компетенцию" : "Добавить компетенцию"}</DialogTitle></DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="code">Код</Label>
              <Input id="code" value={formData.code} onChange={(e) => setFormData({ ...formData, code: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="name">Название</Label>
              <Input id="name" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="description">Описание</Label>
              <textarea id="description" value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} className="w-full px-4 py-2 rounded-lg border border-border bg-background text-foreground" rows={3} />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setIsDialogOpen(false)}>Отмена</Button>
            <Button onClick={handleSave} className="bg-primary text-primary-foreground">{editingCompetency ? "Сохранить" : "Добавить"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
