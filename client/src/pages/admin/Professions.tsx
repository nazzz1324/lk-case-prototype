import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Plus, Edit, Trash2 } from "lucide-react";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { api } from "@/lib/api";

export default function Professions() {
  const [professions, setProfessions] = useState<any[]>([]);
  const [competencies, setCompetencies] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingProfession, setEditingProfession] = useState<any>(null);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    competencies: [] as number[]
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [profData, compData] = await Promise.all([
          api.admin.getProfessions(),
          api.admin.getCompetencies(),
        ]);
        setProfessions(profData);
        setCompetencies(compData);
      } catch (error) {
        console.error("Failed to fetch professions:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  const handleAddEdit = (profession: any = null) => {
    setEditingProfession(profession);
    setFormData({
      name: profession?.name || "",
      description: profession?.description || "",
      competencies: profession?.competencies || []
    });
    setIsDialogOpen(true);
  };

  const handleSave = async () => {
    try {
      // api.admin.saveProfession(formData)
      setIsDialogOpen(false);
      const profData = await api.admin.getProfessions();
      setProfessions(profData);
    } catch (error) {
      console.error("Failed to save profession:", error);
    }
  };

  const handleCompetencyChange = (competencyId: number) => {
    setFormData(prev => {
      if (prev.competencies.includes(competencyId)) {
        return { ...prev, competencies: prev.competencies.filter(c => c !== competencyId) };
      } else {
        return { ...prev, competencies: [...prev.competencies, competencyId] };
      }
    });
  };

  const handleDelete = async (id: number) => {
    if (confirm("Вы уверены?")) {
      setProfessions(professions.filter((p) => p.id !== id));
    }
  };

  const getCompetencyCode = (id: number) => competencies.find(c => c.id === id)?.code || `ID-${id}`;

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Управление Профессиями</h1>
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogTrigger asChild>
            <Button onClick={() => handleAddEdit(null)}>
              <Plus className="mr-2 h-4 w-4" /> Добавить
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{editingProfession ? "Редактировать Профессию" : "Добавить Новую Профессию"}</DialogTitle>
            </DialogHeader>
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="name" className="text-right">Название</Label>
                <Input id="name" value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="description" className="text-right">Описание</Label>
                <Textarea id="description" value={formData.description} onChange={(e) => setFormData({...formData, description: e.target.value})} className="col-span-3" />
              </div>
              <div className="grid grid-cols-4 items-start gap-4">
                <Label className="text-right pt-2">Компетенции</Label>
                <div className="col-span-3 border border-border rounded-lg p-3 max-h-40 overflow-y-auto space-y-2">
                  {competencies.map((comp) => (
                    <label key={comp.id} className="flex items-center gap-2 cursor-pointer">
                      <input
                        type="checkbox"
                        checked={formData.competencies.includes(comp.id)}
                        onChange={() => handleCompetencyChange(comp.id)}
                        className="w-4 h-4 rounded"
                      />
                      <span className="text-sm text-foreground">{comp.code} - {comp.name}</span>
                    </label>
                  ))}
                </div>
              </div>
            </div>
            <Button onClick={handleSave}>Сохранить</Button>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader><CardTitle>Список Профессий</CardTitle></CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[50px]">ID</TableHead>
                <TableHead>Название</TableHead>
                <TableHead>Описание</TableHead>
                <TableHead>Требуемые Компетенции</TableHead>
                <TableHead className="text-right">Действия</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {professions.map((prof) => (
                <TableRow key={prof.id}>
                  <TableCell className="font-medium">{prof.id}</TableCell>
                  <TableCell>{prof.name}</TableCell>
                  <TableCell>{prof.description}</TableCell>
                  <TableCell>{prof.competencies?.map(getCompetencyCode).join(", ")}</TableCell>
                  <TableCell className="text-right">
                    <Button variant="ghost" size="icon" onClick={() => handleAddEdit(prof)} className="mr-2"><Edit className="h-4 w-4" /></Button>
                    <Button variant="ghost" size="icon" onClick={() => handleDelete(prof.id)}><Trash2 className="h-4 w-4 text-red-500" /></Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}
