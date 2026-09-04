import { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  MenuItem,
  Grid,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
} from '@mui/material';
import { PersonAdd, Visibility, VisibilityOff, Email, Lock, Person, Phone } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { registerEmployee } from '../api/authApi.js';

const emptyForm = { fullName: '', email: '', password: '', phoneNumber: '', roleName: '' };

const ROLES = ['Company Admin', 'Manager', 'Salesperson', 'Warehouse Staff', 'Accountant'];

function Team() {
  const [formData, setFormData] = useState(emptyForm);
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [saving, setSaving] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!formData.fullName.trim() || !formData.email.trim() || !formData.password || !formData.roleName) {
      setError('Full name, email, password and role are required.');
      return;
    }

    setSaving(true);
    try {
      // Note: register-employee bhi ek token wapis deta hai, lekin ye
      // NAYE employee ka token hai — isay auth store mein save NAHI karte,
      // warna aap (Company Admin) apne hi session se logout ho kar
      // naye employee ke session mein chale jaayenge.
      await registerEmployee(formData);
      setSuccess(`${formData.fullName} added successfully as ${formData.roleName}.`);
      setFormData(emptyForm);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to add team member.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Typography variant="h4" fontWeight={700} gutterBottom>
          Add Team Member
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
          Add a Manager, Salesperson, Warehouse Staff or Accountant to your organization
        </Typography>

        <Paper elevation={0} sx={{ p: 4, borderRadius: 3, maxWidth: 600 }}>
          {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}
          {success && <Alert severity="success" sx={{ mb: 3 }}>{success}</Alert>}

          <form onSubmit={handleSubmit}>
            <Grid container spacing={2.5}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Full Name"
                  name="fullName"
                  value={formData.fullName}
                  onChange={handleChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start"><Person sx={{ color: 'text.secondary', fontSize: 20 }} /></InputAdornment>
                    ),
                  }}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Email"
                  name="email"
                  type="email"
                  value={formData.email}
                  onChange={handleChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start"><Email sx={{ color: 'text.secondary', fontSize: 20 }} /></InputAdornment>
                    ),
                  }}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.password}
                  onChange={handleChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start"><Lock sx={{ color: 'text.secondary', fontSize: 20 }} /></InputAdornment>
                    ),
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton onClick={() => setShowPassword(!showPassword)} edge="end">
                          {showPassword ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Phone Number (optional)"
                  name="phoneNumber"
                  value={formData.phoneNumber}
                  onChange={handleChange}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start"><Phone sx={{ color: 'text.secondary', fontSize: 20 }} /></InputAdornment>
                    ),
                  }}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  select
                  label="Role"
                  name="roleName"
                  value={formData.roleName}
                  onChange={handleChange}
                >
                  {ROLES.map((role) => (
                    <MenuItem key={role} value={role}>{role}</MenuItem>
                  ))}
                </TextField>
              </Grid>

              <Grid item xs={12}>
                <Button
                  type="submit"
                  variant="contained"
                  size="large"
                  startIcon={saving ? null : <PersonAdd />}
                  disabled={saving}
                  sx={{ py: 1.5, px: 4 }}
                >
                  {saving ? <CircularProgress size={22} color="inherit" /> : 'Add Team Member'}
                </Button>
              </Grid>
            </Grid>
          </form>
        </Paper>
      </motion.div>
    </DashboardLayout>
  );
}

export default Team;