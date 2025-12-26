import { useState, useEffect } from "react";
import { ChevronDown, ChevronUp, Loader2 } from "lucide-react";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface CompetenceIndicator {
  id: number;
  index: string;
  name: string;
  score: number; // 0-5
}

interface CompetenceData {
  name: string;
  indicators: CompetenceIndicator[];
  score: number; // прогресс 0-100%
}

interface CompetenceListItem {
  id: number;
  index: string;
  name: string;
  progress: number; // прогресс 0-100%
}

export default function StudentCompetencies() {
  const [studentId, setStudentId] = useState<number | null>(null);
  const [competences, setCompetences] = useState<CompetenceListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedCompetence, setExpandedCompetence] = useState<number | null>(null);
  const [competenceDetails, setCompetenceDetails] = useState<Record<number, CompetenceData>>({});
  const [loadingDetails, setLoadingDetails] = useState<Record<number, boolean>>({});

  useEffect(() => {
    // Получаем studentId
    const storedId = localStorage.getItem('userId') || 
                    sessionStorage.getItem('userId') ||
                    localStorage.getItem('studentId') ||
                    sessionStorage.getItem('studentId');
    
    console.log("📋 Найденный studentId:", storedId);
    
    if (storedId) {
      setStudentId(Number(storedId));
    } else {
      toast.error("ID студента не найден");
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (studentId) {
      fetchStudentCompetences();
    }
  }, [studentId]);

  // ИЗМЕНЕНИЕ 1: Используем правильный метод
  const fetchStudentCompetences = async () => {
    if (!studentId) return;

    try {
      setLoading(true);
      console.log("📤 Запрашиваем компетенции СТУДЕНТА:", studentId);
      
      // ИЗМЕНЕНИЕ 2: Используем метод getStudentCompetences(studentId)
      const response = await apiClient.getStudentCompetences(studentId);
      console.log("📥 Ответ с компетенциями студента:", response);
      
      if (response.data && response.isSuccess) {
        // Теперь response.data уже содержит прогресс!
        const competencesList: CompetenceListItem[] = response.data.map((c: any) => ({
          id: c.id,
          index: c.index || c.code || `ID-${c.id}`,
          name: c.name,
          progress: c.progress || 0 // ← УЖЕ ЕСТЬ ПРОГРЕСС!
        }));
        
        console.log("📊 Список компетенций с прогрессом:", competencesList);
        setCompetences(competencesList);
      } else {
        console.error("Ошибка:", response.errorMessage);
        toast.error(response.errorMessage || "Не удалось загрузить компетенции");
      }
    } catch (error: any) {
      console.error("Failed to fetch student competences:", error);
      toast.error(error.message || "Не удалось загрузить компетенции");
    } finally {
      setLoading(false);
    }
  };

  const fetchCompetenceScores = async (competenceId: number) => {
    if (!studentId) {
      toast.error("ID студента не найден");
      return;
    }

    if (competenceDetails[competenceId]) {
      console.log("✅ Данные уже загружены для competenceId:", competenceId);
      return;
    }

    try {
      setLoadingDetails(prev => ({ ...prev, [competenceId]: true }));
      console.log("📤 Запрашиваем детали компетенции:", {
        studentId,
        competenceId,
        url: `/Student/getCompetenceScores?studentId=${studentId}&competenceId=${competenceId}`
      });
      
      const response = await apiClient.getStudentCompetenceScores(studentId, competenceId);
      console.log("📥 Ответ с деталями компетенции:", response);
      
      if (response.data && response.isSuccess) {
        const data = response.data;
        console.log("📊 Детали компетенции:", {
          name: data.name,
          indicatorsCount: data.indicators?.length || 0,
          score: data.score
        });
        
        // Сохраняем детали (прогресс уже есть в списке!)
        setCompetenceDetails(prev => ({
          ...prev,
          [competenceId]: {
            name: data.name,
            indicators: data.indicators || [],
            score: data.score || 0
          }
        }));
      } else {
        console.error("Ошибка:", response.errorMessage);
        toast.error(response.errorMessage || "Не удалось загрузить детали");
      }
    } catch (error: any) {
      console.error(`Failed to fetch scores for competence ${competenceId}:`, error);
      toast.error(error.message || "Не удалось загрузить детали");
    } finally {
      setLoadingDetails(prev => ({ ...prev, [competenceId]: false }));
    }
  };

  const handleToggle = (competenceId: number) => {
    if (expandedCompetence === competenceId) {
      setExpandedCompetence(null);
    } else {
      setExpandedCompetence(competenceId);
      fetchCompetenceScores(competenceId);
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

  const getScoreColor = (score: number) => {
    if (score >= 4) return "text-green-600";
    if (score === 3) return "text-yellow-600";
    return "text-red-600";
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
        <span className="ml-2 text-lg text-muted-foreground">Загрузка компетенций...</span>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-foreground mb-2">Освоение компетенций</h2>
        <p className="text-muted-foreground">Просмотр прогресса по индикаторам для каждой компетенции</p>
      </div>

      <div className="space-y-4">
        {competences.length > 0 ? (
          competences.map((competence) => (
            <div key={competence.id} className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
              {/* Header */}
              <button
                onClick={() => handleToggle(competence.id)}
                className="w-full px-6 py-4 flex items-center justify-between hover:bg-secondary/50 transition-colors"
              >
                <div className="text-left flex-1">
                  <h3 className="text-lg font-semibold text-foreground">
                    {competence.index} - {competence.name}
                  </h3>
                </div>
                <div className="flex items-center gap-6">
                  <div className="text-right">
                    <p className="text-xs text-muted-foreground uppercase tracking-wider">Прогресс</p>
                    <p className={`text-2xl font-bold ${getProgressColor(competence.progress)}`}>
                      {competence.progress}%
                    </p>
                  </div>
                  <div className="w-24 h-2 bg-secondary rounded-full overflow-hidden">
                    <div
                      className={`h-full transition-all ${getProgressBarColor(competence.progress)}`}
                      style={{ width: `${competence.progress}%` }}
                    />
                  </div>
                  {expandedCompetence === competence.id ? (
                    <ChevronUp className="text-primary" size={24} />
                  ) : (
                    <ChevronDown className="text-muted-foreground" size={24} />
                  )}
                </div>
              </button>

              {/* Expanded Content */}
              {expandedCompetence === competence.id && (
                <div className="border-t border-border px-6 py-6 bg-secondary/20">
                  {loadingDetails[competence.id] ? (
                    <div className="flex justify-center py-4">
                      <Loader2 className="w-6 h-6 animate-spin text-primary" />
                      <span className="ml-2 text-muted-foreground">Загрузка индикаторов...</span>
                    </div>
                  ) : competenceDetails[competence.id] ? (
                    <div className="space-y-4">
                      {competenceDetails[competence.id].indicators.length > 0 ? (
                        competenceDetails[competence.id].indicators.map((indicator) => (
                          <div key={indicator.id} className="flex items-center justify-between p-4 bg-card rounded-xl border border-border shadow-sm">
                            <div className="flex-1 pr-4">
                              <p className="font-medium text-foreground">
                                {indicator.index} - {indicator.name}
                              </p>
                            </div>
                            <div className="text-right min-w-[100px]">
                              <p className="text-xs text-muted-foreground mb-1">Оценка (1-5)</p>
                              <span className={`inline-flex items-center justify-center w-10 h-10 rounded-lg font-bold text-xl ${getScoreColor(indicator.score)}`}>
                                {indicator.score}
                              </span>
                            </div>
                          </div>
                        ))
                      ) : (
                        <p className="text-center text-muted-foreground py-2">
                          Нет оценок по индикаторам этой компетенции
                        </p>
                      )}
                    </div>
                  ) : (
                    <p className="text-center text-muted-foreground py-2">
                      Загружаем индикаторы...
                    </p>
                  )}
                </div>
              )}
            </div>
          ))
        ) : (
          <div className="text-center py-12 bg-card rounded-2xl border border-dashed border-border">
            <p className="text-muted-foreground">Компетенции не найдены</p>
          </div>
        )}
      </div>
    </div>
  );
}