import { useState, useEffect } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";
import { api } from "@/lib/api";

export default function StudentCompetencies() {
  const [expandedCompetency, setExpandedCompetency] = useState<number | null>(null);
  const [competencies, setCompetencies] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const studentId = parseInt(sessionStorage.getItem("userId") || "1");

  useEffect(() => {
    const fetchCompetencies = async () => {
      try {
        const data = await api.student.getCompetencies(studentId);
        setCompetencies(data);
      } catch (error) {
        console.error("Failed to fetch student competencies:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchCompetencies();
  }, [studentId]);

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-foreground mb-2">Освоение компетенций</h2>
        <p className="text-muted-foreground">Просмотр прогресса по индикаторам для каждой компетенции</p>
      </div>

      <div className="space-y-4">
        {competencies.map((competency) => (
          <div key={competency.id} className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
            <button
              onClick={() => setExpandedCompetency(expandedCompetency === competency.id ? null : competency.id)}
              className="w-full px-6 py-4 flex items-center justify-between hover:bg-secondary transition-colors"
            >
              <div className="text-left">
                <h3 className="text-lg font-semibold text-foreground">{competency.code} - {competency.name}</h3>
                <p className="text-sm text-muted-foreground">{competency.description}</p>
              </div>
              <div className="flex items-center gap-4">
                <div className="text-right">
                  <p className="text-sm text-muted-foreground">Прогресс</p>
                  <p className="text-2xl font-bold text-primary">{competency.progress || 0}%</p>
                </div>
                {expandedCompetency === competency.id ? <ChevronUp className="text-primary" /> : <ChevronDown className="text-muted-foreground" />}
              </div>
            </button>

            {expandedCompetency === competency.id && (
              <div className="border-t border-border px-6 py-4 bg-secondary/30">
                <div className="space-y-3">
                  {competency.indicators?.map((indicator: any) => (
                    <div key={indicator.id} className="flex items-center justify-between p-3 bg-card rounded-lg border border-border">
                      <div>
                        <p className="font-medium text-foreground">{indicator.code} - {indicator.name}</p>
                        <p className="text-xs text-muted-foreground">Уровень: {indicator.level}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-sm text-muted-foreground">Прогресс</p>
                        <p className="text-2xl font-bold text-primary">{indicator.progress || 0}%</p>
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
