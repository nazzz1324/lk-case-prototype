import { useState, useEffect } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";
import { api } from "@/lib/api";

const convertGradeToFivePoint = (grade: number): number => {
  if (grade >= 90) return 5;
  if (grade >= 75) return 4;
  if (grade >= 60) return 3;
  if (grade >= 40) return 2;
  return 1;
};

export default function StudentDisciplines() {
  const [expandedDiscipline, setExpandedDiscipline] = useState<number | null>(null);
  const [disciplines, setDisciplines] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const studentId = parseInt(sessionStorage.getItem("userId") || "1");

  useEffect(() => {
    const fetchDisciplines = async () => {
      try {
        const data = await api.student.getDisciplines(studentId);
        setDisciplines(data);
      } catch (error) {
        console.error("Failed to fetch student disciplines:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchDisciplines();
  }, [studentId]);

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-foreground mb-2">Мои дисциплины и оценки</h2>
        <p className="text-muted-foreground">Просмотр оценок по индикаторам для каждой дисциплины</p>
      </div>

      <div className="space-y-4">
        {disciplines.map((discipline) => (
          <div key={discipline.id} className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
            <button
              onClick={() => setExpandedDiscipline(expandedDiscipline === discipline.id ? null : discipline.id)}
              className="w-full px-6 py-4 flex items-center justify-between hover:bg-secondary transition-colors"
            >
              <div className="text-left">
                <h3 className="text-lg font-semibold text-foreground">{discipline.name}</h3>
                <p className="text-sm text-muted-foreground">Семестр {discipline.semester}</p>
              </div>
              <div className="flex items-center gap-4">
                <div className="text-right">
                  <p className="text-sm text-muted-foreground">Средняя оценка</p>
                  <p className="text-2xl font-bold text-primary">{discipline.averageGrade || 0}</p>
                </div>
                {expandedDiscipline === discipline.id ? <ChevronUp className="text-primary" /> : <ChevronDown className="text-muted-foreground" />}
              </div>
            </button>

            {expandedDiscipline === discipline.id && (
              <div className="border-t border-border px-6 py-4 bg-secondary/30">
                <div className="space-y-3">
                  {discipline.indicators?.map((indicator: any) => (
                    <div key={indicator.id} className="flex items-center justify-between p-3 bg-card rounded-lg border border-border">
                      <div>
                        <p className="font-medium text-foreground">{indicator.code} - {indicator.name}</p>
                        <p className="text-xs text-muted-foreground">Уровень: {indicator.level}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-sm text-muted-foreground">Оценка (1-5)</p>
                        <p className="text-2xl font-bold text-primary">{convertGradeToFivePoint(indicator.grade || 0)}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
