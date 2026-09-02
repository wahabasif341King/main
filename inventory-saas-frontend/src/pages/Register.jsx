import { useState } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Link,
  Alert,
  InputAdornment,
  IconButton,
  CircularProgress,
  Grid,
} from '@mui/material';
import { Visibility, VisibilityOff, Email, Lock, Business, Person } from '@mui/icons-material';
import { motion } from 'framer-motion';
import AnimatedBackground from '../components/AnimatedBackground.jsx';
import { registerOrganization } from '../api/authApi.js';
import useAuthStore from '../store/authStore.js';

function Register() {
  const navigate = useNavigate();
  const loginSuccess = useAuthStore((state) => state.loginSuccess);

  const [formData, setFormData] = useState({
    organizationName: '',
    adminFullName: '',
    adminEmail: '',
    password: '',
    currency: 'PKR',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await registerOrganization(formData);
      loginSuccess(response.data);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ position: 'relative', minHeight: '100vh', overflow: 'hidden' }}>
      <AnimatedBackground />

      <Box
        sx={{
          position: 'relative',
          zIndex: 1,
          minHeight: '100vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: 2,
          py: 4,
        }}
      >
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, ease: 'easeOut' }}
        >
          <Paper
            elevation={0}
            sx={{
              width: { xs: '90vw', sm: 460 },
              padding: 5,
              borderRadius: 4,
            }}
          >
            <Typography variant="h4" fontWeight={700} gutterBottom>
              Create Your Business
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Set up your organization and start managing inventory
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 3 }}>
                {error}
              </Alert>
            )}

            <form onSubmit={handleSubmit}>
              <TextField
                fullWidth
                label="Organization Name"
                name="organizationName"
                value={formData.organizationName}
                onChange={handleChange}
                required
                sx={{ mb: 3 }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Business sx={{ color: 'text.secondary', fontSize: 20 }} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Your Full Name"
                name="adminFullName"
                value={formData.adminFullName}
                onChange={handleChange}
                required
                sx={{ mb: 3 }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Person sx={{ color: 'text.secondary', fontSize: 20 }} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Email"
                name="adminEmail"
                type="email"
                value={formData.adminEmail}
                onChange={handleChange}
                required
                sx={{ mb: 3 }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Email sx={{ color: 'text.secondary', fontSize: 20 }} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Password"
                name="password"
                type={showPassword ? 'text' : 'password'}
                value={formData.password}
                onChange={handleChange}
                required
                sx={{ mb: 4 }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Lock sx={{ color: 'text.secondary', fontSize: 20 }} />
                    </InputAdornment>
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

              <Button
                fullWidth
                type="submit"
                variant="contained"
                size="large"
                disabled={loading}
                sx={{ py: 1.5, fontSize: '1rem' }}
              >
                {loading ? <CircularProgress size={24} color="inherit" /> : 'Create Account'}
              </Button>
            </form>

            <Typography variant="body2" color="text.secondary" sx={{ mt: 3, textAlign: 'center' }}>
              Already have an account?{' '}
              <Link component={RouterLink} to="/login" sx={{ fontWeight: 600 }}>
                Login
              </Link>
            </Typography>
          </Paper>
        </motion.div>
      </Box>
    </Box>
  );
}

export default Register;