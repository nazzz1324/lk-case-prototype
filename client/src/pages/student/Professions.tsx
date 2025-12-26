import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { Users, Target, Loader2 } from "lucide-react";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface StudentProfession {
  id: number;
  name: string;
  description: string | null;
  score: number | null; // прогресс 0-100%
  competencies: number[]; // ID компетенций
  completedCompetenciesCount: number; // завершено компетенций
}

interface Competence {
  id: number;
  index: string;
  name: string;
  progress: number; // прогресс 0-100%
}

export default function StudentProfessions() {
  const [studentId, setStudentId] = useState<number | null>(null);
  const [profession, setProfession] = useState<StudentProfession | null>(null);
  const [competences, setCompetences] = useState<Competence[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingCompetences, setLoadingCompetences] = useState(false);

  useEffect(() => {
    // Получаем studentId
    const storedId = localStorage.getItem('userId') || 
                    sessionStorage.getItem('userId') ||
                    localStorage.getItem('studentId') ||
                    sessionStorage.getItem('studentId');
    
    if (storedId) {
      setStudentId(Number(storedId));
    } else {
      toast.error("ID студента не найден");
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (studentId) {
      fetchStudentProfession();
    }
  }, [studentId]);

  const fetchStudentProfession = async () => {
    if (!studentId) return;

    try {
      setLoading(true);
      console.log("📤 Запрашиваем профроль студента:", studentId);
      
      // Используем метод getStudentProles (в единственном числе, т.к. у студента одна профроль)
      const response = await apiClient.getStudentProles(studentId);
      console.log("📥 Ответ с профролью:", response);
      
      if (response.data && response.isSuccess) {
        setProfession(response.data);
        
        // Загружаем компетенции для отображения подробностей
        if (response.data.competencies?.length > 0) {
          fetchCompetencesDetails(response.data.competencies);
        }
      } else {
        console.error("Ошибка:", response.errorMessage);
        toast.error(response.errorMessage || "Не удалось загрузить профроль");
      }
    } catch (error: any) {
      console.error("Failed to fetch student profession:", error);
      toast.error(error.message || "Не удалось загрузить профроль");
    } finally {
      setLoading(false);
    }
  };

  const fetchCompetencesDetails = async (competenceIds: number[]) => {
    if (!studentId) return;

    try {
      setLoadingCompetences(true);
      
      // Для каждой компетенции получаем детали и прогресс
      const competencesDetails: Competence[] = [];
      
      for (const competenceId of competenceIds) {
        try {
          const response = await apiClient.getStudentCompetenceScores(studentId, competenceId);
          
          if (response.data && response.isSuccess) {
            const competence = response.data;
            competencesDetails.push({
              id: competenceId,
              index: competence.name.split(' ')[0] || `ID-${competenceId}`, // или используй реальный индекс если есть
              name: competence.name,
              progress: competence.score
            });
          }
        } catch (error) {
          console.error(`Ошибка загрузки компетенции ${competenceId}:`, error);
        }
      }
      
      setCompetences(competencesDetails);
    } catch (error) {
      console.error("Failed to fetch competences details:", error);
    } finally {
      setLoadingCompetences(false);
    }
  };

  // Функции для цветов прогресса
  const getProgressColor = (progress: number) => {
    if (progress >= 75) return "text-green-600";
    if (progress >= 50) return "text-yellow-600";
    return "text-red-600";
  };

  const getProgressBarColor = (progress: number) => {
    if (progress >= 75) return "bg-green-500";
    if (progress >= 50) return "bg-yellow-500";
    return "bg-red-500";
  };

  if (!studentId) {
    return (
      <div className="flex items-center justify-center h-64">
        <p className="text-lg text-muted-foreground">Необходима авторизация</p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
        <span className="ml-2 text-lg text-muted-foreground">Загрузка профроли...</span>
      </div>
    );
  }

  if (!profession) {
    return (
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Мой Прогресс по Профессии</h1>
          <p className="text-muted-foreground mt-2">
            Здесь вы можете увидеть, насколько вы готовы к освоению выбранной профессии на основе ваших компетенций.
          </p>
        </div>
        <Card className="text-center py-12">
          <p className="text-muted-foreground">Профессиональная роль не назначена</p>
          <p className="text-sm text-muted-foreground mt-2">
            Обратитесь к администратору для назначения профроли
          </p>
        </Card>
      </div>
    );
  }

  const progress = profession.score || 0;
  const totalCompetencies = profession.competencies?.length || 0;
  const completedCount = profession.completedCompetenciesCount || 0;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-foreground">Мой Прогресс по Профессии</h1>
        <p className="text-muted-foreground mt-2">
          Здесь вы можете увидеть, насколько вы готовы к освоению выбранной профессии на основе ваших компетенций.
        </p>
      </div>

      <Card key={profession.id} className="hover:shadow-lg transition-shadow">
        <CardHeader className="space-y-2">
          <div className="flex items-start justify-between">
            <CardTitle className="text-xl font-semibold">{profession.name}</CardTitle>
            <Users className="h-6 w-6 text-muted-foreground" />
          </div>
          <CardDescription className="text-sm">
            {profession.description || "Описание профессии отсутствует"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className={`text-2xl font-bold mb-4 ${getProgressColor(progress)}`}>
            {progress}% Готовность
          </div>
          <div className="w-full h-2 bg-secondary rounded-full overflow-hidden mb-1">
            <div
              className={`h-full transition-all ${getProgressBarColor(progress)}`}
              style={{ width: `${progress}%` }}
            />
          </div>
          <p className="text-xs text-muted-foreground mt-2">
            Освоено {completedCount} из {totalCompetencies} требуемых компетенций.
          </p>
          
          {totalCompetencies > 0 && (
            <div className="mt-4 p-3 bg-secondary/50 rounded-lg">
              <h3 className="text-sm font-semibold mb-2 text-foreground">Требуемые компетенции:</h3>
              
              {loadingCompetences ? (
                <div className="flex justify-center py-4">
                  <Loader2 className="w-5 h-5 animate-spin text-primary" />
                  <span className="ml-2 text-sm text-muted-foreground">Загрузка компетенций...</span>
                </div>
              ) : competences.length > 0 ? (
                <ul className="text-sm space-y-2">
                  {competences.map((competence) => (
                    <li key={competence.id} className="flex items-center justify-between p-2 bg-card rounded border border-border">
                      <div className="flex items-center gap-2">
                        <Target className="h-3 w-3 text-primary flex-shrink-0" />
                        <div>
                          <span className="font-medium text-foreground">
                            {competence.index}
                          </span>
                          <span className="text-muted-foreground ml-1">
                            ({competence.name})
                          </span>
                        </div>
                      </div>
                      <span className={`font-bold text-sm ${getProgressColor(competence.progress)}`}>
                        {competence.progress}%
                      </span>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-center text-sm text-muted-foreground py-2">
                  Загружаем информацию о компетенциях...
                </p>
              )}
            </div>
          )}

          {totalCompetencies === 0 && (
            <div className="mt-4 p-3 bg-secondary/50 rounded-lg text-center">
              <p className="text-sm text-muted-foreground">Компетенции не назначены</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}