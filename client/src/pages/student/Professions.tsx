import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { Users, Target } from "lucide-react";
import { api } from "@/lib/api";

export default function StudentProfessions() {
  const [professions, setProfessions] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const studentId = parseInt(sessionStorage.getItem("userId") || "1");

  useEffect(() => {
    const fetchProfessions = async () => {
      try {
        // В реальном API может быть отдельный эндпоинт для профессий студента
        const data = await api.admin.getProfessions();
        setProfessions(data);
      } catch (error) {
        console.error("Failed to fetch student professions:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfessions();
  }, [studentId]);

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Мой Прогресс по Профессиям</h1>
      <p className="text-muted-foreground">
        Здесь вы можете увидеть, насколько вы готовы к освоению выбранных профессий на основе ваших компетенций.
      </p>

      <div className="grid gap-6 md:grid-cols-2">
        {professions.map((prof, index) => (
          <Card key={index}>
            <CardHeader className="flex flex-col space-y-1 pb-2">
              <div className="flex items-center justify-between">
                <CardTitle className="text-xl font-medium">{prof.name}</CardTitle>
                <Users className="h-6 w-6 text-muted-foreground" />
              </div>
              <CardDescription className="text-sm text-muted-foreground">{prof.description}</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold mb-4">{prof.progress || 0}% Готовность</div>
              <Progress value={prof.progress || 0} className="h-2" />
              <p className="text-xs text-muted-foreground mt-2">
                Освоено {prof.achievedCompetencies || 0} из {prof.requiredCompetencies || 0} требуемых компетенций.
              </p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
