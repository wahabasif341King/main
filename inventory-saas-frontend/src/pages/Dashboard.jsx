import { Box, Grid, Paper, Typography, Avatar } from '@mui/material';
import {
  Inventory2,
  ShoppingCart,
  Warning,
  TrendingUp,
} from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import useAuthStore from '../store/authStore.js';

const stats = [
  { label: 'Total Products', value: '0', icon: <Inventory2 />, color: '#6366F1' },
  { label: 'Orders Today', value: '0', icon: <ShoppingCart />, color: '#EC4899' },
  { label: 'Low Stock Items', value: '0', icon: <Warning />, color: '#F59E0B' },
  { label: 'Sales Today', value: 'Rs. 0', icon: <TrendingUp />, color: '#10B981' },
];

function StatCard({ stat, index }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, delay: index * 0.1 }}
    >
      <Paper
        elevation={0}
        sx={{
          p: 3,
          borderRadius: 3,
          display: 'flex',
          alignItems: 'center',
          gap: 2,
          transition: 'transform 0.2s ease',
          '&:hover': { transform: 'translateY(-4px)' },
        }}
      >
        <Avatar
          sx={{
            bgcolor: `${stat.color}22`,
            color: stat.color,
            width: 52,
            height: 52,
          }}
        >
          {stat.icon}
        </Avatar>
        <Box>
          <Typography variant="h5" fontWeight={700}>
            {stat.value}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {stat.label}
          </Typography>
        </Box>
      </Paper>
    </motion.div>
  );
}

function Dashboard() {
  const user = useAuthStore((state) => state.user);

  return (
    <DashboardLayout>
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ duration: 0.5 }}
      >
        <Typography variant="h4" fontWeight={700} gutterBottom>
          Welcome back, {user?.fullName?.split(' ')[0] || 'there'} 👋
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Here's what's happening with your business today.
        </Typography>

        <Grid container spacing={3}>
          {stats.map((stat, index) => (
            <Grid item xs={12} sm={6} md={3} key={stat.label}>
              <StatCard stat={stat} index={index} />
            </Grid>
          ))}
        </Grid>

        <Paper elevation={0} sx={{ mt: 4, p: 4, borderRadius: 3, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary">
            More modules (Products, Orders, Inventory...) coming soon 🚀
          </Typography>
        </Paper>
      </motion.div>
    </DashboardLayout>
  );
}

export default Dashboard;