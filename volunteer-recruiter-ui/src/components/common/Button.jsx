import React from 'react';

/**
 * `Button` component serves as a reusable button with customizable styles and actions.
 * 
 * Props:
 * - `onClick` (function): Callback function triggered when the button is clicked.
 * - `children` (node): The content or elements to display inside the button.
 * - `className` (string, optional): Additional CSS classes for custom styling.
 * - `...props` (object): Additional properties passed to the button element.
 * 
 * Usage:
 * ```jsx
 * <Button onClick={handleClick} className="custom-class">Click Me</Button>
 * ```
 */

const Button = ({ onClick, children, className = "", ...props }) => {
  return (
    <button onClick={onClick} className={className} {...props}>
      {children}
    </button>
  );
};

export default Button;

