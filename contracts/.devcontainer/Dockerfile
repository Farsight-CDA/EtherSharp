FROM ghcr.io/foundry-rs/foundry:stable
USER root
RUN apt-get update && apt-get install -y \
    wget \
    tar \
    coreutils \
    curl \
    && curl -fsSL https://deb.nodesource.com/setup_current.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*