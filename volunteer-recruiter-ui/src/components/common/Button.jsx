import React from 'react';

const Button = ({ onClick, children, className = "", ...props }) => {
  return (
    <button onClick={onClick} className={className} {...props}>
      {children}
    </button>
  );
};

export default Button;

