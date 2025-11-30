/**
 * Shared UI utilities and mixins
 */

import { designTokens } from './design-tokens';

export const uiHelpers = {
  /**
   * Get button styles by variant
   */
  getButtonStyles: (variant: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'ghost') => {
    const baseStyles = `
      font-family: ${designTokens.typography.fontFamily.sans};
      font-size: ${designTokens.typography.fontSize.base};
      font-weight: ${designTokens.typography.fontWeight.semibold};
      padding: ${designTokens.spacing.sm} ${designTokens.spacing.lg};
      border: none;
      border-radius: ${designTokens.borderRadius.md};
      cursor: pointer;
      transition: all ${designTokens.transition.base};
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: ${designTokens.spacing.sm};
      outline: none;
    `;

    const variantStyles: Record<string, string> = {
      primary: `
        background-color: ${designTokens.colors.primary[500]};
        color: white;
        box-shadow: ${designTokens.shadow.base};
        &:hover { background-color: ${designTokens.colors.primary[600]}; box-shadow: ${designTokens.shadow.md}; }
        &:active { background-color: ${designTokens.colors.primary[700]}; }
        &:disabled { background-color: ${designTokens.colors.neutral[300]}; cursor: not-allowed; }
      `,
      secondary: `
        background-color: ${designTokens.colors.secondary[500]};
        color: white;
        box-shadow: ${designTokens.shadow.base};
        &:hover { background-color: ${designTokens.colors.secondary[600]}; box-shadow: ${designTokens.shadow.md}; }
        &:active { background-color: ${designTokens.colors.secondary[700]}; }
        &:disabled { background-color: ${designTokens.colors.neutral[300]}; cursor: not-allowed; }
      `,
      success: `
        background-color: ${designTokens.colors.success[500]};
        color: white;
        box-shadow: ${designTokens.shadow.base};
        &:hover { background-color: ${designTokens.colors.success[600]}; box-shadow: ${designTokens.shadow.md}; }
        &:active { background-color: ${designTokens.colors.success[700]}; }
        &:disabled { background-color: ${designTokens.colors.neutral[300]}; cursor: not-allowed; }
      `,
      warning: `
        background-color: ${designTokens.colors.warning[500]};
        color: white;
        box-shadow: ${designTokens.shadow.base};
        &:hover { background-color: ${designTokens.colors.warning[600]}; box-shadow: ${designTokens.shadow.md}; }
        &:active { background-color: ${designTokens.colors.warning[700]}; }
        &:disabled { background-color: ${designTokens.colors.neutral[300]}; cursor: not-allowed; }
      `,
      error: `
        background-color: ${designTokens.colors.error[500]};
        color: white;
        box-shadow: ${designTokens.shadow.base};
        &:hover { background-color: ${designTokens.colors.error[600]}; box-shadow: ${designTokens.shadow.md}; }
        &:active { background-color: ${designTokens.colors.error[700]}; }
        &:disabled { background-color: ${designTokens.colors.neutral[300]}; cursor: not-allowed; }
      `,
      ghost: `
        background-color: transparent;
        color: ${designTokens.colors.primary[500]};
        border: 1px solid ${designTokens.colors.primary[300]};
        &:hover { background-color: ${designTokens.colors.primary[50]}; }
        &:active { background-color: ${designTokens.colors.primary[100]}; }
        &:disabled { color: ${designTokens.colors.neutral[400]}; border-color: ${designTokens.colors.neutral[300]}; }
      `,
    };

    return baseStyles + variantStyles[variant];
  },

  /**
   * Get input styles
   */
  getInputStyles: () => `
    font-family: ${designTokens.typography.fontFamily.sans};
    font-size: ${designTokens.typography.fontSize.base};
    padding: ${designTokens.spacing.sm} ${designTokens.spacing.md};
    border: 1px solid ${designTokens.colors.neutral[300]};
    border-radius: ${designTokens.borderRadius.md};
    transition: all ${designTokens.transition.base};
    
    &:focus {
      outline: none;
      border-color: ${designTokens.colors.primary[500]};
      box-shadow: 0 0 0 3px ${designTokens.colors.primary[100]};
    }
    
    &:disabled {
      background-color: ${designTokens.colors.neutral[100]};
      color: ${designTokens.colors.neutral[400]};
      cursor: not-allowed;
    }
  `,

  /**
   * Get card styles
   */
  getCardStyles: () => `
    background-color: white;
    border-radius: ${designTokens.borderRadius.lg};
    border: 1px solid ${designTokens.colors.neutral[200]};
    box-shadow: ${designTokens.shadow.base};
    padding: ${designTokens.spacing.lg};
    transition: all ${designTokens.transition.base};
    
    &:hover {
      box-shadow: ${designTokens.shadow.md};
      border-color: ${designTokens.colors.neutral[300]};
    }
  `,

  /**
   * Get badge styles
   */
  getBadgeStyles: (variant: 'primary' | 'success' | 'warning' | 'error' = 'primary') => {
    const colors: Record<string, { bg: string; text: string }> = {
      primary: { bg: designTokens.colors.primary[100], text: designTokens.colors.primary[700] },
      success: { bg: designTokens.colors.success[100], text: designTokens.colors.success[700] },
      warning: { bg: designTokens.colors.warning[100], text: designTokens.colors.warning[700] },
      error: { bg: designTokens.colors.error[100], text: designTokens.colors.error[700] },
    };
    const color = colors[variant];
    return `
      background-color: ${color.bg};
      color: ${color.text};
      padding: ${designTokens.spacing.xs} ${designTokens.spacing.md};
      border-radius: ${designTokens.borderRadius.full};
      font-size: ${designTokens.typography.fontSize.xs};
      font-weight: ${designTokens.typography.fontWeight.semibold};
      display: inline-block;
    `;
  },
};
