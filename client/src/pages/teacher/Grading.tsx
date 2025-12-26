import { useState, useEffect } from "react";
import { Save } from "lucide-react";
import { Button } from "@/components/ui/button";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface Indicator {
  id: number;
  index: string;
  name: string;
}

interface Student {
  id: number;
  fullName: string;
  scores: (number | null)[];
}

interface ScoringData {
  disciplineName: string;
  groupName: string;
  students: Student[];
  indicators: Indicator[];
}

interface GradeState {
  [studentId: number]: {
    [indicatorIndex: number]: number;
  };
}

export default function Grading() {
  const [disciplines, setDisciplines] = useState<any[]>([]);
  const [groups, setGroups] = useState<any[]>([]);
  const [scoringData, setScoringData] = useState<ScoringData | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedDisciplineId, setSelectedDisciplineId] = useState<number | null>(null);
  const [selectedGroupId, setSelectedGroupId] = useState<number | null>(null);
  const [grades, setGrades] = useState<GradeState>({});
  const [saving, setSaving] = useState(false);

  const teacherId = parseInt(sessionStorage.getItem("userId") || "1");

  useEffect(() => {
    fetchInitialData();
  }, []);

  useEffect(() => {
    if (selectedDisciplineId && selectedGroupId) {
      fetchScoringData();
    }
  }, [selectedDisciplineId, selectedGroupId]);

  const fetchInitialData = async () => {
    try {
      setLoading(true);
      const [disciplinesRes, groupsRes] = await Promise.all([
        apiClient.getTeacherDisciplines(teacherId),
        apiClient.getGroups(),
      ]);

      if (disciplinesRes.data) {
        setDisciplines(disciplinesRes.data);
        if (disciplinesRes.data.length > 0) {
          setSelectedDisciplineId(disciplinesRes.data[0].id);
        }
      }
      
      if (groupsRes.data) {
        setGroups(groupsRes.data);
        if (groupsRes.data.length > 0) {
          setSelectedGroupId(groupsRes.data[0].id);
        }
      }
    } catch (error) {
      console.error("Failed to fetch initial data:", error);
      toast.error("Не удалось загрузить данные");
    } finally {
      setLoading(false);
    }
  };

  const fetchScoringData = async () => {
    if (!selectedDisciplineId || !selectedGroupId) return;

    try {
      setLoading(true);
      const response = await apiClient.getScoringData({
        disciplineId: selectedDisciplineId,
        groupId: selectedGroupId,
        teacherId: teacherId,
      });

      if (response.isSuccess && response.data) {
        setScoringData(response.data);
        
        const initialGrades: GradeState = {};
        response.data.students.forEach((student: Student) => {
          initialGrades[student.id] = {};
          student.scores.forEach((score: number | null, index: number) => {
            if (score !== null) {
              initialGrades[student.id][index] = score;
            }
          });
        });
        setGrades(initialGrades);
      }
    } catch (error) {
      console.error("Failed to fetch scoring data:", error);
      toast.error("Не удалось загрузить данные для оценивания");
    } finally {
      setLoading(false);
    }
  };

  const handleGradeChange = (studentId: number, indicatorIndex: number, value: number) => {
    setGrades((prev) => ({
      ...prev,
      [studentId]: {
        ...(prev[studentId] || {}),
        [indicatorIndex]: value,
      },
    }));
  };

  const getGrade = (studentId: number, indicatorIndex: number): number => {
    return grades[studentId]?.[indicatorIndex] ?? 0;
  };

  const calculateFinalGrade = (studentId: number): number => {
    if (!scoringData || scoringData.indicators.length === 0) {
      return 0;
    }

    const totalIndicators = scoringData.indicators.length;
    const totalSum = scoringData.indicators.reduce((sum, _, index) => {
      return sum + getGrade(studentId, index);
    }, 0);

    const average = totalSum / totalIndicators;
    return Math.round(average * 10) / 10;
  };

  const handleSave = async () => {
    if (!selectedDisciplineId || !scoringData) {
      toast.error("Выберите дисциплину и группу");
      return;
    }

    try {
      setSaving(true);
      
      const scores = [];
      for (const [studentIdStr, indicatorGrades] of Object.entries(grades)) {
        const studentId = parseInt(studentIdStr);
        for (const [indicatorIndexStr, score] of Object.entries(indicatorGrades)) {
          const indicatorIndex = parseInt(indicatorIndexStr);
          const indicator = scoringData.indicators[indicatorIndex];
          
          if (indicator) {
            scores.push({
              studentId: studentId,
              indicatorId: indicator.id,
              score: score,
            });
          }
        }
      }

      const response = await apiClient.saveScores({
        disciplineId: selectedDisciplineId,
        teacherId: teacherId,
        scores: scores,
      });

      if (response.isSuccess) {
        toast.success("Оценки успешно сохранены");
      } else {
        toast.error(response.errorMessage || "Не удалось сохранить оценки");
      }
    } catch (error) {
      console.error("Failed to save scores:", error);
      toast.error("Не удалось сохранить оценки");
    } finally {
      setSaving(false);
    }
  };

  if (loading && !scoringData) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-lg text-muted-foreground">Загрузка...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="bg-card rounded-2xl p-6 shadow-sm border border-border">
        <h3 className="text-lg font-semibold text-foreground mb-4">
          Выбор дисциплины и группы
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">Дисциплина</label>
            <select
              value={selectedDisciplineId || ""}
              onChange={(e) => setSelectedDisciplineId(parseInt(e.target.value))}
              className="w-full px-4 py-2 rounded-lg border border-border bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            >
              {disciplines.map((discipline) => (
                <option key={discipline.id} value={discipline.id}>
                  {discipline.name}
                </option>
              ))}
            </select>
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground">Группа</label>
            <select
              value={selectedGroupId || ""}
              onChange={(e) => setSelectedGroupId(parseInt(e.target.value))}
              className="w-full px-4 py-2 rounded-lg border border-border bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
            >
              {groups.map((group) => (
                <option key={group.id} value={group.id}>
                  {group.name}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {scoringData && (
        <>
          <div className="bg-card rounded-2xl shadow-sm border border-border overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-border bg-secondary">
                    <th className="px-6 py-4 text-left text-sm font-semibold text-foreground sticky left-0 bg-secondary z-10">
                      Студент
                    </th>
                    {scoringData.indicators.map((indicator, index) => (
                      <th
                        key={indicator.id}
                        className="px-6 py-4 text-center text-sm font-semibold text-foreground min-w-[180px]"
                      >
                        <div className="flex flex-col items-center">
                          <span className="font-bold">{indicator.index}</span>
                          <span className="text-xs font-normal text-muted-foreground mt-1">
                            {indicator.name.length > 30
                              ? indicator.name.substring(0, 30) + "..."
                              : indicator.name}
                          </span>
                        </div>
                      </th>
                    ))}
                    <th className="px-6 py-4 text-center text-sm font-semibold text-foreground bg-primary/10">
                      Итоговая оценка
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {scoringData.students.map((student) => (
                    <tr
                      key={student.id}
                      className="border-b border-border hover:bg-secondary transition-colors"
                    >
                      <td className="px-6 py-4 text-sm font-medium text-foreground sticky left-0 bg-card">
                        {student.fullName}
                      </td>
                      {scoringData.indicators.map((indicator, index) => (
                        <td key={indicator.id} className="px-6 py-4">
                          <input
                            type="number"
                            min="0"
                            max="100"
                            value={getGrade(student.id, index) || ""}
                            onChange={(e) =>
                              handleGradeChange(
                                student.id,
                                index,
                                parseInt(e.target.value) || 0
                              )
                            }
                            className="w-full px-3 py-2 text-center rounded-lg border border-border bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
                            placeholder="0"
                          />
                        </td>
                      ))}
                      <td className="px-6 py-4 text-center">
                        <span className="inline-flex items-center justify-center px-4 py-2 rounded-lg bg-primary/10 text-primary font-semibold">
                          {calculateFinalGrade(student.id)}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          <div className="flex justify-end">
            <Button
              onClick={handleSave}
              disabled={saving}
              className="bg-primary hover:bg-primary/90 text-primary-foreground"
            >
              <Save size={18} className="mr-2" />
              {saving ? "Сохранение..." : "Сохранить изменения"}
            </Button>
          </div>
        </>
      )}

      {!loading && scoringData && scoringData.students.length === 0 && (
        <div className="text-center py-12">
          <p className="text-muted-foreground">
            Студенты не найдены в выбранной группе
          </p>
        </div>
      )}
    </div>
  );
}
