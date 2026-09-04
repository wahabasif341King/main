import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Grid,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
} from '@mui/material';
import { Add, Edit, Delete, AccountTree } from '@mui/icons-material';
import ProductVariantsDialog from '../components/ProductVariantsDialog.jsx';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import {
  getProducts,
  createProduct,
  updateProduct,
  deleteProduct,
  getCategories,
  getBrands,
  getTaxes,
} from '../api/catalogApi.js';

const emptyForm = {
  name: '',
  sku: '',
  barcode: '',
  description: '',
  categoryId: '',
  brandId: '',
  taxId: '',
  unitOfMeasure: 'Piece',
  costPrice: '',
  sellingPrice: '',
  minimumStock: '',
  maximumStock: '',
  reorderLevel: '',
};

function Products() {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [taxes, setTaxes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const [saving, setSaving] = useState(false);
  const [variantsProduct, setVariantsProduct] = useState(null); // jis product ke variants dekhne hain

  const loadAll = async () => {
    setLoading(true);
    try {
      const [productsRes, categoriesRes, brandsRes, taxesRes] = await Promise.all([
        getProducts(),
        getCategories(),
        getBrands(),
        getTaxes(),
      ]);
      setProducts(productsRes.data);
      setCategories(categoriesRes.data);
      setBrands(brandsRes.data);
      setTaxes(taxesRes.data);
    } catch (err) {
      setError('Failed to load products.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAll();
  }, []);

  const handleOpenCreate = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setError('');
    setOpen(true);
  };

  const handleOpenEdit = (p) => {
    setEditingId(p.productId);
    setFormData({
      name: p.name || '',
      sku: p.sku || '',
      barcode: p.barcode || '',
      description: p.description || '',
      categoryId: p.categoryId || '',
      brandId: p.brandId || '',
      taxId: p.taxId || '',
      unitOfMeasure: p.unitOfMeasure || 'Piece',
      costPrice: p.costPrice ?? '',
      sellingPrice: p.sellingPrice ?? '',
      minimumStock: p.minimumStock ?? '',
      maximumStock: p.maximumStock ?? '',
      reorderLevel: p.reorderLevel ?? '',
    });
    setError('');
    setOpen(true);
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async () => {
    if (!formData.name.trim() || !formData.sku.trim() || !formData.categoryId) {
      setError('Name, SKU and Category are required.');
      return;
    }
    setSaving(true);
    setError('');
    const payload = {
      name: formData.name,
      sku: formData.sku,
      barcode: formData.barcode || null,
      description: formData.description || null,
      categoryId: formData.categoryId,
      brandId: formData.brandId || null,
      taxId: formData.taxId || null,
      unitOfMeasure: formData.unitOfMeasure,
      costPrice: parseFloat(formData.costPrice) || 0,
      sellingPrice: parseFloat(formData.sellingPrice) || 0,
      minimumStock: parseInt(formData.minimumStock) || 0,
      maximumStock: parseInt(formData.maximumStock) || 0,
      reorderLevel: parseInt(formData.reorderLevel) || 0,
    };
    try {
      if (editingId) {
        await updateProduct(editingId, payload);
      } else {
        await createProduct(payload);
      }
      setOpen(false);
      loadAll();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save product. Check if SKU is unique.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this product?')) return;
    try {
      await deleteProduct(id);
      loadAll();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to delete product.');
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Products</Typography>
            <Typography variant="body2" color="text.secondary">Manage your product catalog</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpenCreate}>
            Add Product
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>SKU</TableCell>
                  <TableCell>Category</TableCell>
                  <TableCell>Cost</TableCell>
                  <TableCell>Selling Price</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={7} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : products.length === 0 ? (
                  <TableRow><TableCell colSpan={7} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No products yet. Add your first one.</Typography></TableCell></TableRow>
                ) : (
                  products.map((p) => (
                    <TableRow key={p.productId} hover>
                      <TableCell>{p.name}</TableCell>
                      <TableCell>{p.sku}</TableCell>
                      <TableCell>{p.categoryName || '—'}</TableCell>
                      <TableCell>Rs. {p.costPrice}</TableCell>
                      <TableCell>Rs. {p.sellingPrice}</TableCell>
                      <TableCell>
                        <Chip label={p.status} size="small" color={p.status === 'Active' ? 'success' : 'default'} />
                      </TableCell>
                      <TableCell align="right">
                        <IconButton size="small" onClick={() => setVariantsProduct(p)} title="Manage Variants">
                            <AccountTree fontSize="small" />
                        </IconButton>
                        <IconButton size="small" onClick={() => handleOpenEdit(p)}>
                            <Edit fontSize="small" />
                        </IconButton>
                        <IconButton size="small" color="error" onClick={() => handleDelete(p.productId)}>
                            <Delete fontSize="small" />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </motion.div>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="md">
        <DialogTitle>{editingId ? 'Edit Product' : 'Add Product'}</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12} sm={6}>
              <TextField fullWidth label="Product Name" name="name" value={formData.name} onChange={handleChange} required />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField fullWidth label="SKU" name="sku" value={formData.sku} onChange={handleChange} required />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField fullWidth label="Barcode (optional)" name="barcode" value={formData.barcode} onChange={handleChange} />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField fullWidth label="Unit of Measure" name="unitOfMeasure" value={formData.unitOfMeasure} onChange={handleChange} />
            </Grid>

            <Grid item xs={12}>
              <TextField fullWidth label="Description (optional)" name="description" value={formData.description} onChange={handleChange} multiline rows={2} />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField fullWidth select label="Category" name="categoryId" value={formData.categoryId} onChange={handleChange} required>
                {categories.map((c) => (
                  <MenuItem key={c.categoryId} value={c.categoryId}>{c.name}</MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField fullWidth select label="Brand (optional)" name="brandId" value={formData.brandId} onChange={handleChange}>
                <MenuItem value="">None</MenuItem>
                {brands.map((b) => (
                  <MenuItem key={b.brandId} value={b.brandId}>{b.name}</MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField fullWidth select label="Tax (optional)" name="taxId" value={formData.taxId} onChange={handleChange}>
                <MenuItem value="">None</MenuItem>
                {taxes.map((t) => (
                  <MenuItem key={t.taxId} value={t.taxId}>{t.name} ({t.percentage}%)</MenuItem>
                ))}
              </TextField>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Cost Price"
                name="costPrice"
                type="number"
                value={formData.costPrice}
                onChange={handleChange}
                InputProps={{ startAdornment: <InputAdornment position="start">Rs.</InputAdornment> }}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Selling Price"
                name="sellingPrice"
                type="number"
                value={formData.sellingPrice}
                onChange={handleChange}
                InputProps={{ startAdornment: <InputAdornment position="start">Rs.</InputAdornment> }}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField fullWidth label="Minimum Stock" name="minimumStock" type="number" value={formData.minimumStock} onChange={handleChange} />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField fullWidth label="Maximum Stock" name="maximumStock" type="number" value={formData.maximumStock} onChange={handleChange} />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField fullWidth label="Reorder Level" name="reorderLevel" type="number" value={formData.reorderLevel} onChange={handleChange} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 1 }}>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? <CircularProgress size={20} color="inherit" /> : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
      <ProductVariantsDialog
        open={Boolean(variantsProduct)}
        onClose={() => setVariantsProduct(null)}
        product={variantsProduct}
      />
    </DashboardLayout>
  );
}

export default Products;