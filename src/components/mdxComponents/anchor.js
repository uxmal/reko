import * as React from "react";

const AnchorTag = ({ children: link, ...props }) => {
  if(link) {
    return (
      <a href={props.href} target="_blank" rel="noopener">{link}</a>
    );
  } else {
    return null;
  }
};

export default AnchorTag;
