import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  IconButton,
  Alert,
  CircularProgress,
  Divider,
} from '@mui/material';
import { Add, Edit, Delete, Close } from '@mui/icons-material';
import {
  getVariantsByProduct,
  createProductVariant,
  updateProductVariant,
  deleteProductVariant,
} from '../api/catalogApi.js';

const emptyForm = { color: '', size: '' };

// Product ki base SKU + Color + Size se automatically ek unique variant SKU bana deta hai
// (backend ko SKU chahiye hoti hai, lekin user se manually nahi mangwate)
function generateVariantSku(baseSku, color, size) {
  const parts = [baseSku, color, size].filter(Boolean).map((p) => p.trim().toUpperCase().replace(/\s+/g, ''));
  const suffix = Date.now().toString().slice(-4); // duplicate combos se bachne ke liye
  return [...parts, suffix].join('-');
}

function ProductVariantsDialog({ open, onClose, product }) {
  const [variants, setVariants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [editingId, setEditingId] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState(emptyForm);
  const [saving, setSaving] = useState(false);

  const loadVariants = async () => {
    if (!product) return;
    setLoading(true);
    try {
      const res = await getVariantsByProduct(product.productId);
      setVariants(res.data);
    } catch (err) {
      setError('Failed to load variants.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (open) {
      loadVariants();
      setShowForm(false);
      setEditingId(null);
      setError('');
    }
  }, [open, product]);

  const handleOpenAddForm = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setError('');
    setShowForm(true);
  };

  const handleOpenEditForm = (variant) => {
    setEditingId(variant.variantId);
    setFormData({ color: variant.color || '', size: variant.size || '' });
    setError('');
    setShowForm(true);
  };

  const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSave = async () => {
    if (!formData.color.trim() && !formData.size.trim()) {
      setError('Enter at least a Color or a Size.');
      return;
    }
    setSaving(true);
    setError('');

    // Edit karte waqt purani SKU hi rakhte hain (backend ko phir bhi SKU chahiye,
    // lekin user ko dikhani/badalni nahi hai)
    const existingVariant = variants.find((v) => v.variantId === editingId);
    const sku = existingVariant ? existingVariant.sku : generateVariantSku(product.sku, formData.color, formData.size);

    const payload = {
      productId: product.productId,
      sku,
      color: formData.color || null,
      size: formData.size || null,
      price: existingVariant ? existingVariant.price : 0,
      cost: existingVariant ? existingVariant.cost : 0,
    };
    try {
      if (editingId) {
        await updateProductVariant(editingId, payload);
      } else {
        await createProductVariant(payload);
      }
      setShowForm(false);
      loadVariants();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save variant.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this variant?')) return;
    try {
      await deleteProductVariant(id);
      loadVariants();
    } catch (err) {
      setError('Failed to delete variant.');
    }
  };

  if (!product) return null;

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h6">Variants — {product.name}</Typography>
        <IconButton onClick={onClose} size="small"><Close /></IconButton>
      </DialogTitle>

      <DialogContent>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <TableContainer sx={{ mb: 2 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Color</TableCell>
                <TableCell>Size</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={3} align="center" sx={{ py: 3 }}><CircularProgress size={24} /></TableCell></TableRow>
              ) : variants.length === 0 ? (
                <TableRow><TableCell colSpan={3} align="center" sx={{ py: 3 }}><Typography color="text.secondary" variant="body2">No variants yet.</Typography></TableCell></TableRow>
              ) : (
                variants.map((v) => (
                  <TableRow key={v.variantId} hover>
                    <TableCell>{v.color || '—'}</TableCell>
                    <TableCell>{v.size || '—'}</TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => handleOpenEditForm(v)}>
                        <Edit fontSize="small" />
                      </IconButton>
                      <IconButton size="small" color="error" onClick={() => handleDelete(v.variantId)}>
                        <Delete fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>

        {!showForm ? (
          <Button startIcon={<Add />} onClick={handleOpenAddForm}>
            Add Variant
          </Button>
        ) : (
          <Box sx={{ p: 2, border: '1px solid rgba(255,255,255,0.08)', borderRadius: 2 }}>
            <Typography variant="subtitle2" sx={{ mb: 2 }}>
              {editingId ? 'Edit Variant' : 'New Variant'}
            </Typography>
            <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 2 }}>
              <TextField label="Color" name="color" value={formData.color} onChange={handleChange} size="small" sx={{ width: 160 }} />
              <TextField label="Size" name="size" value={formData.size} onChange={handleChange} size="small" sx={{ width: 160 }} />
            </Box>
            <Button variant="contained" size="small" onClick={handleSave} disabled={saving} sx={{ mr: 1 }}>
              {saving ? <CircularProgress size={18} color="inherit" /> : 'Save'}
            </Button>
            <Button size="small" onClick={() => setShowForm(false)}>Cancel</Button>
          </Box>
        )}
      </DialogContent>

      <Divider />
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
}

export default ProductVariantsDialog;