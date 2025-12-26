import { useState, useEffect } from "react";
import { ChevronDown, ChevronUp, Loader2 } from "lucide-react";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface IndicatorScore {
  id: number;
  index: string;
  name: string;
  score: number;
}

interface DisciplineScoreData {
  name: string;
  indicators: IndicatorScore[];
  disciplineScore: number;
}

interface StudentDiscipline {
  id: number;
  name: string;
  teacherName: string;
  score: number;
}

export default function StudentDisciplines() {
  const [studentId, setStudentId] = useState<number | null>(null);
  const [disciplines, setDisciplines] = useState<StudentDiscipline[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedDiscipline, setExpandedDiscipline] = useState<number | null>(null);
  const [disciplineDetails, setDisciplineDetails] = useState<Record<number, DisciplineScoreData>>({});
  const [loadingDetails, setLoadingDetails] = useState<Record<number, boolean>>({});

  useEffect(() => {
    // Получаем studentId из localStorage (сохранён при логине)
    const storedId = localStorage.getItem('studentId');
    if (storedId) {
      setStudentId(Number(storedId));
    } else {
      toast.error("ID студента не найден. Пожалуйста, войдите снова.");
    }
  }, []);

  useEffect(() => {
    if (studentId) {
      fetchDisciplines();
    }
  }, [studentId]);

  const fetchDisciplines = async () => {
    if (!studentId) return;

    try {
      setLoading(true);
      const response = await apiClient.getStudentDisciplines(studentId);
      if (response.data) {
        setDisciplines(response.data);
      }
    } catch (error) {
      console.error("Failed to fetch student disciplines:", error);
      toast.error("Не удалось загрузить список дисциплин");
    } finally {
      setLoading(false);
    }
  };

  const fetchDisciplineScore = async (disciplineId: number) => {
    if (!studentId || disciplineDetails[disciplineId]) return;

    try {
      setLoadingDetails(prev => ({ ...prev, [disciplineId]: true }));
      const response = await apiClient.getStudentDisciplineScore(studentId, disciplineId);
      if (response.data) {
        setDisciplineDetails(prev => ({ ...prev, [disciplineId]: response.data }));
      }
    } catch (error) {
      console.error(`Failed to fetch score for discipline ${disciplineId}:`, error);
      toast.error("Не удалось загрузить оценки по дисциплине");
    } finally {
      setLoadingDetails(prev => ({ ...prev, [disciplineId]: false }));
    }
  };

  const handleToggle = (disciplineId: number) => {
    if (expandedDiscipline === disciplineId) {
      setExpandedDiscipline(null);
    } else {
      setExpandedDiscipline(disciplineId);
      fetchDisciplineScore(disciplineId);
    }
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
        <span className="ml-2 text-lg text-muted-foreground">Загрузка дисциплин...</span>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-foreground mb-2">Мои дисциплины и оценки</h2>
        <p className="text-muted-foreground">Просмотр оценок по индикаторам для каждой дисциплины</p>
      </div>

      <div className="space-y-4">
        {disciplines.map((discipline) => (
          <div key={discipline.id} className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
            {/* Header */}
            <button
              onClick={() => handleToggle(discipline.id)}
              className="w-full px-6 py-4 flex items-center justify-between hover:bg-secondary/50 transition-colors"
            >
              <div className="text-left">
                <h3 className="text-lg font-semibold text-foreground">{discipline.name}</h3>
                <p className="text-sm text-muted-foreground">
                  Преподаватель: {discipline.teacherName || "Не назначен"}
                </p>
              </div>
              <div className="flex items-center gap-6">
                <div className="text-right">
                  <p className="text-xs text-muted-foreground uppercase tracking-wider">Средняя оценка</p>
                  <p className="text-2xl font-bold text-primary">{discipline.score || 0}</p>
                </div>
                {expandedDiscipline === discipline.id ? (
                  <ChevronUp className="text-primary" size={24} />
                ) : (
                  <ChevronDown className="text-muted-foreground" size={24} />
                )}
              </div>
            </button>

            {/* Expanded Content */}
            {expandedDiscipline === discipline.id && (
              <div className="border-t border-border px-6 py-6 bg-secondary/20">
                {loadingDetails[discipline.id] ? (
                  <div className="flex justify-center py-4">
                    <Loader2 className="w-6 h-6 animate-spin text-primary" />
                  </div>
                ) : disciplineDetails[discipline.id] ? (
                  <div className="space-y-4">
                    {disciplineDetails[discipline.id].indicators.map((indicator) => (
                      <div key={indicator.id} className="flex items-center justify-between p-4 bg-card rounded-xl border border-border shadow-sm">
                        <div className="flex-1 pr-4">
                          <p className="font-medium text-foreground">
                            {indicator.index} - {indicator.name}
                          </p>
                        </div>
                        <div className="text-right min-w-[100px]">
                          <p className="text-xs text-muted-foreground mb-1">Оценка (1-5)</p>
                          <span className={`inline-flex items-center justify-center w-10 h-10 rounded-lg font-bold text-xl ${
                            indicator.score >= 4 ? "text-primary" :
                            indicator.score === 3 ? "text-yellow-600" :
                            "text-destructive"
                          }`}>
                            {indicator.score}
                          </span>
                        </div>
                      </div>
                    ))}
                    {disciplineDetails[discipline.id].indicators.length === 0 && (
                      <p className="text-center text-muted-foreground py-2">Индикаторы не найдены</p>
                    )}
                  </div>
                ) : (
                  <p className="text-center text-muted-foreground py-2">Не удалось загрузить детали</p>
                )}
              </div>
            )}
          </div>
        ))}
      </div>

      {disciplines.length === 0 && (
        <div className="text-center py-12 bg-card rounded-2xl border border-dashed border-border">
          <p className="text-muted-foreground">У вас пока нет назначенных дисциплин</p>
        </div>
      )}
    </div>
  );
}