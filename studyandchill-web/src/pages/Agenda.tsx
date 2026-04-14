import { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Paper, useTheme, Chip, Button, CircularProgress,
  Dialog, DialogTitle, DialogContent, DialogActions, TextField, MenuItem,
  FormControl, InputLabel, Select
} from '@mui/material';
import Grid from '@mui/material/Grid';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import VideocamIcon from '@mui/icons-material/Videocam';
import PersonIcon from '@mui/icons-material/Person';
import EditIcon from '@mui/icons-material/Edit';
import FactCheckIcon from '@mui/icons-material/FactCheck';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import DeleteIcon from '@mui/icons-material/Delete';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface StudentSummary {
  id: number;
  name: string;
}

interface ClassSession {
  id: number;
  start: string;
  end: string;
  status: number;
  teacherId: number;
  teacherName: string;
  students: StudentSummary[];
  meetingUrl?: string;
}

interface FetchParams {
  start: string;
  end: string;
  studentId?: number;
}

interface Contract {
  status: number;
  makeUpQuota: number;
  classDuration: number;
}

interface TeacherAvailability {
  dayOfWeek: number;
  availableFrom: string;
  availableTo: string;
}

export default function Agenda() {
  const theme = useTheme();
  const navigate = useNavigate();

  const userStr = localStorage.getItem('user');
  const user = userStr ? JSON.parse(userStr) : null;
  const isTeacher = user?.role === 'Teacher' || user?.role === 1;

  const [classes, setClasses] = useState<ClassSession[]>([]);
  const [loading, setLoading] = useState(true);

  const [startDate, setStartDate] = useState(() => {
    const d = new Date();
    d.setDate(d.getDate() - 15);
    return d.toISOString().split('T')[0];
  });
  const [endDate, setEndDate] = useState(() => {
    const d = new Date();
    d.setDate(d.getDate() + 30);
    return d.toISOString().split('T')[0];
  });
  const [myStudents, setMyStudents] = useState<StudentSummary[]>([]);
  const [filterStudentId, setFilterStudentId] = useState<number | ''>('');

  const [selectedClassId, setSelectedClassId] = useState<number | null>(null);
  const [linkDialogOpen, setLinkDialogOpen] = useState(false);
  const [newLink, setNewLink] = useState('');
  const [statusDialogOpen, setStatusDialogOpen] = useState(false);
  const [newStatus, setNewStatus] = useState<number>(1);

  const [scheduleDialogOpen, setScheduleDialogOpen] = useState(false);
  const [schedDate, setSchedDate] = useState('');
  const [schedTime, setSchedTime] = useState('');
  const [schedDuration, setSchedDuration] = useState(60);
  const [schedStudentId, setSchedStudentId] = useState<number | ''>('');
  const [scheduleLoading, setScheduleLoading] = useState(false);

  const [makeUpQuota, setMakeUpQuota] = useState<number | null>(null);
  const [teacherAvails, setTeacherAvails] = useState<TeacherAvailability[]>([]);


  useEffect(() => {
    if (!isTeacher) {
      api.get('/contracts/my-contracts')
        .then(res => {
          const contracts: Contract[] = Array.isArray(res.data) ? res.data : [res.data];
          const active = contracts.find(c => c.status === 1) || contracts[0];
          if (active) {
            setMakeUpQuota(active.makeUpQuota);
            setSchedDuration(active.classDuration || 60);
          }
        })
        .catch(e => console.error("Erro ao buscar quota", e));

      api.get('/teacheravailability/makeup')
        .then(res => setTeacherAvails(res.data))
        .catch(e => console.error("Erro ao buscar horários", e));
    }
  }, [isTeacher]);

  useEffect(() => {
    if (isTeacher) {
      api.get('/users/my-students')
        .then(res => setMyStudents(res.data))
        .catch(err => console.error('Erro ao buscar alunos', err));
    }
  }, [isTeacher]);

  const fetchClasses = useCallback(async () => {
    try {
      setLoading(true);
      const startIso = new Date(`${startDate}T00:00:00`).toISOString();
      const endIso = new Date(`${endDate}T23:59:59`).toISOString();

      const params: FetchParams = { start: startIso, end: endIso };
      if (filterStudentId !== '') params.studentId = filterStudentId;

      const response = await api.get('/classsession', { params });
      setClasses(response.data);
    } catch (error) {
      console.error('Erro ao buscar aulas:', error);
    } finally {
      setLoading(false);
    }
  }, [startDate, endDate, filterStudentId]);

  useEffect(() => {
    fetchClasses();
  }, [fetchClasses]);

  const handleSaveLink = async () => {
    if (!selectedClassId) return;
    try {
      await api.put(`/classsession/${selectedClassId}/link`, { meetingUrl: newLink });
      setLinkDialogOpen(false);
      fetchClasses();
    } catch (error) {
      alert('Erro ao salvar o link.');
    }
  };

  const handleSaveStatus = async () => {
    if (!selectedClassId) return;
    try {
      await api.put(`/classsession/${selectedClassId}/status`, { newStatus: newStatus });
      setStatusDialogOpen(false);
      fetchClasses();
    } catch (error: unknown) {
      const err = error as { response?: { data?: string } };
      alert(err.response?.data || 'Erro ao atualizar status.');
    }
  };

  const handleScheduleSubmit = async () => {
    if (!schedDate || !schedTime || (isTeacher && schedStudentId === '')) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    try {
      setScheduleLoading(true);
      const startDateTime = new Date(`${schedDate}T${schedTime}:00`);

      if (isTeacher) {
        await api.post('/classsession/single', {
          studentId: schedStudentId,
          startTime: startDateTime.toISOString(),
          durationInMinutes: schedDuration
        });
        alert('Aula agendada com sucesso!');
      } else {
        await api.post('/classsession/makeup', {
          startTime: startDateTime.toISOString(),
          duration: schedDuration
        });
        alert('Reposição agendada com sucesso!');
        if (makeUpQuota !== null) setMakeUpQuota(makeUpQuota - 1);
      }

      setScheduleDialogOpen(false);
      fetchClasses();
    } catch (error: unknown) {
      const err = error as { response?: { data?: string } };
      alert(err.response?.data || 'Erro ao agendar aula.');
    } finally {
      setScheduleLoading(false);
    }
  };

  const handleDeleteClass = async (id: number) => {
    if (!window.confirm("Tem certeza que deseja desmarcar/apagar esta aula?")) return

    try {
      await api.delete(`/classsession/${id}`);
      fetchClasses();
    } catch (error) {
      console.error('Erro ao excluir aula:', error)
      alert('Erro ao desmarcar a aula.')
    }
  };

  let availableTimeSlots: string[] = [];

  if (schedDate) {
    const now = new Date();
    const todayStr = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
    const currentTotalMins = now.getHours() * 60 + now.getMinutes();

    const baseSlots: string[] = [];
    for (let h = 7; h <= 23; h++) {
      for (let m = 0; m < 60; m += 15) {
        if (schedDate === todayStr && (h * 60 + m) <= currentTotalMins) continue;
        baseSlots.push(`${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`);
      }
    }

    availableTimeSlots = baseSlots;

    if (!isTeacher) {
      availableTimeSlots = [];
      const dateObj = new Date(`${schedDate}T00:00:00`);
      const dayOfWeek = dateObj.getDay();

      const dayAvails = teacherAvails.filter(a => a.dayOfWeek === dayOfWeek);

      dayAvails.forEach(av => {
        const startTotalMins = parseInt(av.availableFrom.split(':')[0]) * 60 + parseInt(av.availableFrom.split(':')[1]);
        const endTotalMins = parseInt(av.availableTo.split(':')[0]) * 60 + parseInt(av.availableTo.split(':')[1]);
        const maxStartTotalMins = endTotalMins - schedDuration;

        baseSlots.forEach(slot => {
          const slotMins = parseInt(slot.split(':')[0]) * 60 + parseInt(slot.split(':')[1]);
          if (slotMins >= startTotalMins && slotMins <= maxStartTotalMins) {
            availableTimeSlots.push(slot);
          }
        });
      });

      availableTimeSlots = [...new Set(availableTimeSlots)].sort();
    }
  }

  const getStatusConfig = (status: number) => {
    switch (status) {
      case 0: return { label: 'Agendada', color: 'primary' as const };
      case 1: return { label: 'Concluída', color: 'success' as const };
      case 4: return { label: 'Falta (S/ Reposição)', color: 'error' as const };
      case 5: return { label: 'Falta (C/ Reposição)', color: 'warning' as const };
      case 6: return { label: 'Reposta', color: 'info' as const };
      default: return { label: 'Desconhecido', color: 'default' as const };
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 4, flexWrap: 'wrap', gap: 2 }}>
        <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', flexGrow: 1 }}>
          <Typography variant="h4" sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
            Minha Agenda
          </Typography>
          <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3 }}>
            Voltar
          </Button>
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          {!isTeacher && makeUpQuota !== null && (
            <Chip
              label={`Créditos de Reposição: ${makeUpQuota}`}
              color="secondary"
              variant="filled"
              sx={{ fontWeight: 'bold' }}
            />
          )}
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddCircleOutlineIcon />}
            sx={{ borderRadius: 3 }}
            onClick={() => {
              setSchedDate('');
              setSchedTime('');
              setSchedStudentId('');
              setScheduleDialogOpen(true);
            }}
          >
            {isTeacher ? 'Nova Aula' : 'Agendar Reposição'}
          </Button>
        </Box>
      </Box>

      <Paper elevation={0} sx={{ p: 2, mb: 4, borderRadius: 2, border: `1px solid ${theme.palette.divider}`, display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center' }}>
        <Typography variant="body1" sx={{ fontWeight: 'bold', color: theme.palette.text.secondary, display: 'flex', alignItems: 'center', gap: 1 }}>
          <FilterAltIcon /> Filtros:
        </Typography>

        <TextField
          label="De (Data Inicial)" type="date" size="small"
          value={startDate} onChange={(e) => setStartDate(e.target.value)}
          sx={{ minWidth: 150 }} slotProps={{ inputLabel: { shrink: true } }}
        />

        <TextField
          label="Até (Data Final)" type="date" size="small"
          value={endDate} onChange={(e) => setEndDate(e.target.value)}
          sx={{ minWidth: 150 }} slotProps={{ inputLabel: { shrink: true } }}
        />

        {isTeacher && (
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel>Filtrar por Aluno</InputLabel>
            <Select
              value={filterStudentId} label="Filtrar por Aluno"
              onChange={(e) => setFilterStudentId(e.target.value as number | '')}
            >
              <MenuItem value=""><em>Todos os Alunos</em></MenuItem>
              {myStudents.map((student) => (
                <MenuItem key={student.id} value={student.id}>{student.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
        )}

        <Button variant="contained" onClick={() => fetchClasses()} sx={{ borderRadius: 2 }}>Buscar</Button>
      </Paper>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}><CircularProgress /></Box>
      ) : classes.length === 0 ? (
        <Paper elevation={0} sx={{ p: 4, textAlign: 'center', bgcolor: 'transparent', border: `1px dashed ${theme.palette.divider}` }}>
          <CalendarTodayIcon sx={{ fontSize: 48, color: theme.palette.text.disabled, mb: 2 }} />
          <Typography variant='h6' color='textSecondary'>Nenhuma aula encontrada para este período.</Typography>
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {classes.map((classItem) => {
            const statusConfig = getStatusConfig(classItem.status);
            const formatedDate = new Date(classItem.start).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' });
            const start = new Date(classItem.start).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

            const cardTitle = isTeacher
              ? classItem.students.map(s => s.name).join(', ') || 'Aluno não atribuído'
              : classItem.teacherName;

            return (
              <Grid size={{ xs: 12, md: 6, lg: 4 }} key={classItem.id}>
                <Paper
                  elevation={0}
                  sx={{
                    p: 3, borderRadius: theme.shape.borderRadius, border: `1px solid ${theme.palette.divider}`,
                    borderTop: classItem.status === 0 ? `4px solid ${theme.palette.primary.main}` : `1px solid ${theme.palette.divider}`,
                    display: 'flex', flexDirection: 'column', gap: 2, transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 2 }
                  }}
                >
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <PersonIcon sx={{ color: theme.palette.primary.main }} />
                      <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.text.primary, lineHeight: 1.2 }}>
                        {cardTitle}
                      </Typography>
                    </Box>
                    <Chip label={statusConfig.label} size="small" color={statusConfig.color} variant={classItem.status === 0 ? 'outlined' : 'filled'} sx={{ fontWeight: 'bold' }} />
                  </Box>

                  <Box sx={{ display: 'flex', gap: 3, color: theme.palette.text.secondary, mt: 1 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <CalendarTodayIcon fontSize="small" />
                      <Typography variant="body2">{formatedDate}</Typography>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                      <AccessTimeIcon fontSize="small" />
                      <Typography variant="body2">{start}</Typography>
                    </Box>
                  </Box>

                  <Box sx={{ mt: 1, display: 'flex', flexDirection: 'column', gap: 1 }}>
                    <Button
                      variant={classItem.meetingUrl ? "contained" : "outlined"} color="primary" fullWidth startIcon={<VideocamIcon />}
                      disabled={!classItem.meetingUrl} sx={{ borderRadius: 2 }}
                      onClick={() => classItem.meetingUrl && window.open(classItem.meetingUrl, '_blank')}
                    >
                      {classItem.meetingUrl ? 'Acessar Aula' : 'Link Indisponível'}
                    </Button>

                    {isTeacher && (
                      <Box sx={{ display: 'flex', gap: 1 }}>
                        <Button
                          variant="outlined" size="small" fullWidth startIcon={<EditIcon />}
                          onClick={() => { setSelectedClassId(classItem.id); setNewLink(classItem.meetingUrl || ''); setLinkDialogOpen(true); }}
                        >
                          Link
                        </Button>
                        <Button
                          variant="outlined" color="success" size="small" fullWidth startIcon={<FactCheckIcon />} disabled={classItem.status !== 0}
                          onClick={() => { setSelectedClassId(classItem.id); setNewStatus(1); setStatusDialogOpen(true); }}
                        >
                          Status
                        </Button>
                        <Button
                          variant="outlined" color="error" size="small"
                          onClick={() => handleDeleteClass(classItem.id)}
                          sx={{ minWidth: '40px', px: 0 }}
                        >
                          <DeleteIcon fontSize="small" />
                        </Button>
                      </Box>
                    )}
                  </Box>
                </Paper>
              </Grid>
            );
          })}
        </Grid>
      )}

      <Dialog open={linkDialogOpen} onClose={() => setLinkDialogOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle sx={{ fontFamily: 'Bodoni Moda', fontWeight: 'bold' }}>Gerenciar Link da Aula</DialogTitle>
        <DialogContent>
          <TextField autoFocus margin="dense" label="URL da Reunião" type="url" fullWidth variant="outlined" value={newLink} onChange={(e) => setNewLink(e.target.value)} />
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setLinkDialogOpen(false)} color="inherit">Cancelar</Button>
          <Button onClick={handleSaveLink} variant="contained" color="primary">Salvar Link</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={statusDialogOpen} onClose={() => setStatusDialogOpen(false)} fullWidth maxWidth="xs">
        <DialogTitle sx={{ fontFamily: 'Bodoni Moda', fontWeight: 'bold' }}>Check-in da Aula</DialogTitle>
        <DialogContent>
          <TextField select fullWidth label="Novo Status" value={newStatus} onChange={(e) => setNewStatus(Number(e.target.value))} sx={{ mt: 2 }}>
            <MenuItem value={1}>✅ Aula Concluída</MenuItem>
            <MenuItem value={5}>⚠️ Falta do Aluno (Com Reposição)</MenuItem>
            <MenuItem value={4}>❌ Falta do Aluno (Sem Reposição)</MenuItem>
          </TextField>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setStatusDialogOpen(false)} color="inherit">Cancelar</Button>
          <Button onClick={handleSaveStatus} variant="contained" color="success">Confirmar Status</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={scheduleDialogOpen} onClose={() => setScheduleDialogOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle sx={{ fontFamily: 'Bodoni Moda', fontWeight: 'bold' }}>
          {isTeacher ? 'Agendar Aula' : 'Solicitar Reposição'}
        </DialogTitle>
        <DialogContent>
          <Typography variant="body2" sx={{ mb: 3, color: theme.palette.text.secondary }}>
            {isTeacher
              ? 'Selecione o aluno e o horário para inserir uma aula na grade.'
              : 'Escolha uma data para ver os horários livres. Atenção: solicite com 5 dias de antecedência.'}
          </Typography>

          <Grid container spacing={2}>
            {isTeacher && (
              <Grid size={{ xs: 12 }}>
                <FormControl fullWidth>
                  <InputLabel>Aluno</InputLabel>
                  <Select
                    value={schedStudentId} label="Aluno"
                    onChange={(e) => setSchedStudentId(e.target.value as number)}
                  >
                    {myStudents.map((student) => (
                      <MenuItem key={student.id} value={student.id}>{student.name}</MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
            )}

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Data da Aula" type="date" fullWidth
                value={schedDate} onChange={(e) => setSchedDate(e.target.value)}
                slotProps={{ inputLabel: { shrink: true } }}
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                select label="Horário" fullWidth
                value={schedTime} onChange={(e) => setSchedTime(e.target.value)}
                disabled={!isTeacher && !schedDate}
              >
                {availableTimeSlots.length === 0 ? (
                  <MenuItem disabled value="">Nenhum horário livre neste dia</MenuItem>
                ) : (
                  availableTimeSlots.map((time) => (
                    <MenuItem key={time} value={time}>{time}</MenuItem>
                  ))
                )}
              </TextField>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setScheduleDialogOpen(false)} color="inherit" disabled={scheduleLoading}>Cancelar</Button>
          <Button onClick={handleScheduleSubmit} variant="contained" color="primary" disabled={scheduleLoading}>
            {scheduleLoading ? <CircularProgress size={24} /> : 'Confirmar Agendamento'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}