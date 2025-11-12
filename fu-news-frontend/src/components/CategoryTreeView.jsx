import { useState, useEffect } from 'react';
import { categoryService } from '../services/categoryService';
import '../styles/CategoryTreeView.css';

export function CategoryTreeView({ onView, onEdit, onDelete, onToggleStatus, onRefresh }) {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [expandedIds, setExpandedIds] = useState(new Set());
  const [error, setError] = useState('');

  useEffect(() => {
    loadRootCategories();
  }, []);

  const loadRootCategories = async () => {
    try {
      setLoading(true);
      const response = await categoryService.getAll();
      const data = response && response.data ? response.data : response;
      
      // Filter chỉ categories root (không có parent hoặc parentId = 0)
      const roots = Array.isArray(data) 
        ? data.filter(c => !c.parentCategoryId || c.parentCategoryId === 0)
        : [];
      
      setCategories(roots);
      setError('');
    } catch (err) {
      console.error('Load root categories error:', err);
      setError('Failed to load categories');
    } finally {
      setLoading(false);
    }
  };

  const loadSubcategories = async (parentId) => {
    try {
      const res = await categoryService.getSubCategories(parentId);
      const subData = res && res.data ? res.data : res;
      
      // Update categories state với subcategories
      setCategories(prev => 
        prev.map(c => 
          c.categoryId === parentId 
            ? { ...c, subCategories: Array.isArray(subData) ? subData : [] }
            : c
        )
      );
    } catch (err) {
      console.error('Load subcategories error:', err);
      setError(`Failed to load subcategories for category ${parentId}`);
    }
  };

  const toggleExpand = async (categoryId, category) => {
    const newExpanded = new Set(expandedIds);
    
    if (newExpanded.has(categoryId)) {
      newExpanded.delete(categoryId);
    } else {
      newExpanded.add(categoryId);
      // Load subcategories nếu chưa load
      if (!category.subCategories) {
        await loadSubcategories(categoryId);
      }
    }
    
    setExpandedIds(newExpanded);
  };

  const renderTreeNode = (category, level = 0) => {
    const hasSubcategories = category.subCategories && category.subCategories.length > 0;
    const isExpanded = expandedIds.has(category.categoryId);

    return (
      <div key={category.categoryId} className="tree-node">
        <div className="tree-node-content" style={{ marginLeft: `${level * 20}px` }}>
          <div className="tree-node-header">
            {hasSubcategories ? (
              <button
                className="tree-expand-btn"
                onClick={() => toggleExpand(category.categoryId, category)}
                title={isExpanded ? 'Collapse' : 'Expand'}
              >
                {isExpanded ? '▼' : '▶'}
              </button>
            ) : (
              <span className="tree-expand-placeholder"></span>
            )}

            <div className="tree-node-info">
              <span className="tree-node-name">{category.categoryName}</span>
              {category.categoryDescrip && (
                <span className="tree-node-desc">{category.categoryDescrip}</span>
              )}
            </div>

            <div className="tree-node-badges">
              <span className={`status-badge ${category.isActive ? 'status-active' : 'status-inactive'}`}>
                {category.isActive ? 'Active' : 'Inactive'}
              </span>
              {category.newsArticleCount > 0 && (
                <span className="count-badge">{category.newsArticleCount} articles</span>
              )}
            </div>

            <div className="tree-node-actions">
              <button
                onClick={() => onView(category)}
                className="btn btn-info"
                title="View details"
              >
                👁
              </button>
              <button
                onClick={() => onEdit(category)}
                className="btn btn-warning"
                title="Edit"
              >
                ✏
              </button>
              <button
                onClick={() => onToggleStatus(category.categoryId)}
                className="btn btn-primary"
                title="Toggle status"
              >
                ◯
              </button>
              <button
                onClick={() => onDelete(category.categoryId)}
                className="btn btn-danger"
                disabled={!category.canDelete}
                title={!category.canDelete ? 'Cannot delete' : 'Delete'}
              >
                🗑
              </button>
            </div>
          </div>

          {isExpanded && hasSubcategories && (
            <div className="tree-node-children">
              {category.subCategories.map(sub => renderTreeNode(sub, level + 1))}
            </div>
          )}
        </div>
      </div>
    );
  };

  if (loading) {
    return <div className="loading">Loading categories...</div>;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  if (!categories || categories.length === 0) {
    return <div className="empty-message">No categories found</div>;
  }

  return (
    <div className="category-tree-container">
      <div className="tree-view">
        {categories.map(category => renderTreeNode(category))}
      </div>
    </div>
  );
}
